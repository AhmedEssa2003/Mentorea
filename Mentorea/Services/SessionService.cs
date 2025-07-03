using Hangfire;
using Mentorea.Abstractions.Enums;
using Mentorea.Contracts.Common;
using Mentorea.Contracts.Session;
using Mentorea.Errors;
using Mentorea.Helpers;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Linq.Dynamic.Core;

namespace Mentorea.Services
{
    public class SessionService(
        MentoreaDbContext context,
        IFcmService fcmService,
        IEmailSender emailSender
    ) : ISessionService
    {
        private readonly MentoreaDbContext _context = context;
        private readonly IFcmService _fcmService = fcmService;
        private readonly IEmailSender _emailSender = emailSender;

        public async Task<Result<PaginatedList<SessionResponse>>> GetAllAsync(RequestFilters filters, CancellationToken cancellationToken)
        {
            var query = _context.Sessions.Include
                (x => x.Mentor)
                .Include(x => x.Mentee)
                .AsNoTracking()
                .OrderBy($"CreatedAt {filters.SortDirection}")
                .Select(x=>new SessionResponse(x.Id,x.MentorId,x.Mentor.Name,x.MenteeId, x.Mentee.Name, x.ScheduledTime, x.DurationMinutes, x.Status.ToString(), x.Notes, x.Price));


            var paginatedResult = await PaginatedList<SessionResponse>.CreateAsync(query, filters.PageNumber, filters.PageSize, cancellationToken);

            return Result.Success(paginatedResult);
        }


        public async Task<Result<SessionResponse>> GetByIdAsync(string id, CancellationToken cancellationToken)
        {
            var session = await _context.Sessions.Include(x => x.Mentor).Include(x => x.Mentee)
                .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (session is null)
                return Result.Failure<SessionResponse>(SessionError.NotFoundSession);
            var response = new SessionResponse(id, session.MentorId,session.Mentor.Name ,session.MenteeId,session.Mentee.Name ,session.ScheduledTime, session.DurationMinutes, session.Status.ToString(), session.Notes, session.Price);
            if (session.Status == SessionStatus.pending)
                response.Price = null;
            return Result.Success(response);
        }

        public async Task<Result<PaginatedList<SessionResponse>>> GetSessionByUserIdWithStatusAsync(string userId, RequestFilters requestFilters, CancellationToken cancellationToken)
        {
            if (!await _context.Users.AnyAsync(x => x.Id == userId && !x.IsDisabled && x.EmailConfirmed, cancellationToken))
                return Result.Failure<PaginatedList<SessionResponse>>(UserError.NotFoundUser);
            var query = _context.Sessions
                .AsNoTracking()
                .Include(x => x.Mentor)
                .Include(x => x.Mentee)
                .Where(x => (x.MentorId == userId || x.MenteeId == userId)
                    && (string.IsNullOrEmpty(requestFilters.SearchValue)
                        || x.Status.ToString() == requestFilters.SearchValue)
                ).OrderBy($"CreatedAt {requestFilters.SortDirection}");

            var src = query.Select(x=> new SessionResponse (
                x.Id, x.MentorId,x.Mentor.Name, x.MenteeId,x.Mentee.Name, x.ScheduledTime, x.DurationMinutes,
                x.Status.ToString(), x.Notes, x.Price));
            var sessions = await PaginatedList<SessionResponse>.CreateAsync(src, requestFilters.PageNumber, requestFilters.PageSize, cancellationToken);
            return Result.Success(sessions);
        }

        public async Task<Result<SessionResponse>> AddAsync(SessionRequest request, string menteeId, CancellationToken cancellationToken)
        {
            if(request.ScheduledTime is null || request.ScheduledTime < DateTime.UtcNow.AddHours(3))
                return Result.Failure<SessionResponse>(SessionError.InvalidScheduledTime);
            
            var hasPendingFeedback = _context.Sessions.Any(s =>
                s.MenteeId == menteeId &&
                s.Status == SessionStatus.awaiting_feedback);

            if (hasPendingFeedback)
                return Result.Failure<SessionResponse>(SessionError.FeedbackRequired);

            if (!await _context.Users.AnyAsync(x => x.Id == request.MentorId && !x.IsDisabled && x.EmailConfirmed, cancellationToken))
                return Result.Failure<SessionResponse>(UserError.NotFoundUser);

            var roleId = await _context.UserRoles
                .Where(x => x.UserId == request.MentorId)
                .Select(x => x.RoleId).FirstAsync(cancellationToken);

            if (roleId != DefaultRole.MentorRoleId)
                return Result.Failure<SessionResponse>(SessionError.UserNotMentor);

            if (await IsSessionTimeConflicting(request.MentorId, request.ScheduledTime!.Value, request.DurationMinutes, cancellationToken))
                return Result.Failure<SessionResponse>(SessionError.TimeTaken);

            var session = request.Adapt<Session>();
            session.Status = SessionStatus.pending;
            session.MenteeId = menteeId;
            session.Price = await _context.Users
                .Where(x => x.Id == request.MentorId)
                .Select(x => x.PriceOfSession)
                .SingleOrDefaultAsync(cancellationToken);

            await _context.Sessions.AddAsync(session, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            BackgroundJob.Enqueue(() => _fcmService.SendPushNotificationToUserDevicesAsync(
                request.MentorId,
                "Session Request: Accept, Decline, or Modify",
                "You have a new session request. Please review and choose to accept, decline, or propose changes.",
                cancellationToken
            ));
            var mentorName = await _context.Users
                .Where(x => x.Id == request.MentorId)
                .Select(x => x.Name)
                .FirstAsync(cancellationToken);
            var menteeName = await _context.Users
                .Where(x => x.Id == request.MenteeId)
                .Select(x => x.Name)
                .FirstAsync(cancellationToken);
            var response = new SessionResponse(session.Id, session.MentorId,mentorName ,session.MenteeId,menteeName ,session.ScheduledTime, session.DurationMinutes, session.Status.ToString(), session.Notes, session.Price);
            return Result.Success(response);
        }

        public async Task<Result> RespondToSessionBeforePayment(string id, ResponseSessionRequest request, CancellationToken cancellationToken)
        {
            var session = await _context.Sessions.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (session is null)
                return Result.Failure(SessionError.NotFoundSession);

            if (session.Status != SessionStatus.pending || session.ScheduledTime < DateTime.UtcNow.AddHours(3))
                return Result.Failure(SessionError.ActionNotAllawed);

            if (request.Status == ApprovalStatus.rejected.ToString())
            {
                session.Status = SessionStatus.rejected;
                BackgroundJob.Enqueue(() => _fcmService.SendPushNotificationToUserDevicesAsync(
                    session.MenteeId,
                    "Session Request Declined",
                    "Your mentor has declined your session request. You can explore other mentors or request another session.",
                    cancellationToken
                ));
            }
            else
            {
                if (request.Price is not null)
                    session.Price = request.Price;
                if (session.Price == 0 || session.Price is null)
                {
                    session.Status = SessionStatus.accepted;
                    var time = session.ScheduledTime.AddMinutes(session.WaitingTime);
                    BackgroundJob.Schedule(() => CheckSessionExpiration(session), time);
                    BackgroundJob.Enqueue(() => _fcmService.SendPushNotificationToUserDevicesAsync(
                        session.MenteeId,
                        "Session Confirmed",
                       "Your session has been confirmed. Check the app for details.",
                        cancellationToken
                    ));
                    await SendSessionConfirmed(session, cancellationToken);
                    BackgroundJob.Enqueue(() => _fcmService.SendPushNotificationToUserDevicesAsync(
                        session.MentorId,
                        "Session Booked",
                       "The session is now confirmed. Check the app for details.",
                        cancellationToken
                    ));
                }
                else
                {
                    BackgroundJob.Enqueue(() => _fcmService.SendPushNotificationToUserDevicesAsync(
                    session.MenteeId,
                    "Session Accepted - Payment Required",
                    "Your mentor has accepted your session request. Please complete the payment to confirm your booking.",
                    cancellationToken
                    ));
                    session.Status = SessionStatus.awaiting_payment;
                }
            }

            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }

        public async Task<Result> ConfirmPaymentAsync(string id, CancellationToken cancellationToken)
        {
            var session = await _context.Sessions.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (session is null)
                return Result.Failure(SessionError.NotFoundSession);

            if (session.Status != SessionStatus.awaiting_payment || session.ScheduledTime < DateTime.UtcNow.AddHours(3))
                return Result.Failure(SessionError.ActionNotAllawed);
            var time = session.ScheduledTime.AddMinutes(session.WaitingTime);
            BackgroundJob.Schedule(() => CheckSessionExpiration(session), time);
            session.Status = SessionStatus.accepted;
            await _context.PymentSessions.AddAsync(new PaymentSession { SessionId = session.Id } ,cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            await SendSessionConfirmed(session, cancellationToken);
            await SendNotification(session, cancellationToken);
            return Result.Success();
        }

        public async Task<Result> UpdateSessionAsync(string id, string userId, UpdateSessionRequest request, CancellationToken cancellationToken)
        {
            var session = await _context.Sessions.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (session is null)
                return Result.Failure(SessionError.NotFoundSession);

            if (session.MentorId != userId)
                return Result.Failure(SessionError.ActionNotAllawed);

            if ((session.Status != SessionStatus.awaiting_payment && session.Status != SessionStatus.pending) || session.ScheduledTime <= DateTime.UtcNow.AddHours(3))
                return Result.Failure(SessionError.ActionNotAllawed);


            if (await IsSessionTimeConflictingForUpdate(id, session.MentorId, request.ScheduledTime!.Value, request.DurationMinutes, cancellationToken))
                return Result.Failure<SessionResponse>(SessionError.TimeTaken);

            session = request.Adapt(session);
            session.Status = SessionStatus.awaiting_mentee_confirmation;
            BackgroundJob.Enqueue(() => _fcmService.SendPushNotificationToUserDevicesAsync(
                session.MenteeId,
                "Mentor Updated Your Session",
                "Your mentor has made changes to the session details. Please log in to review the updated session and decide whether to accept or decline the changes.",
                cancellationToken
            ));
            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }

        public async Task<Result> RespondToUpdateAsync(string id, ResponseUpdateReques request, CancellationToken cancellationToken)
        {
            var session = await _context.Sessions.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (session is null)
                return Result.Failure(SessionError.NotFoundSession);

            if (session.Status != SessionStatus.awaiting_mentee_confirmation || session.ScheduledTime < DateTime.UtcNow.AddHours(3))
                return Result.Failure(SessionError.ActionNotAllawed);

            if (request.Status == ApprovalStatus.rejected.ToString())
            {
                session.Status = SessionStatus.rejected;
                BackgroundJob.Enqueue(() => _fcmService.SendPushNotificationToUserDevicesAsync(
                    session.MentorId,
                    "Mentee Declined Session Update",
                    "Your mentee has declined the changes to the session details. Please check the app for more information.",
                    cancellationToken
                ));

            }
            else
            {
                BackgroundJob.Enqueue(() => _fcmService.SendPushNotificationToUserDevicesAsync(
                    session.MentorId,
                    "Mentee Accepted Session Update",
                    "Your mentee has accepted the changes to the session details. The session is now confirmed with the updated date, time, and price. Please check the app for more information.",
                    cancellationToken
                ));
                session.Status = SessionStatus.awaiting_payment;
            }

            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }

        public async Task<Result> CancelSessionAsync(string id, string userId, CancellationToken cancellationToken)
        {
            var session = await _context.Sessions.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (session is null)
                return Result.Failure(SessionError.NotFoundSession);

            var validStatuses = new[]
            {
                SessionStatus.pending,
                SessionStatus.awaiting_mentee_confirmation,
                SessionStatus.awaiting_payment
            };

            if (!validStatuses.Contains(session.Status) || session.ScheduledTime <= DateTime.UtcNow.AddHours(3))
                return Result.Failure(SessionError.ActionNotAllawed);
            if (session.MentorId == userId)
            {
                BackgroundJob.Enqueue(() => _fcmService.SendPushNotificationToUserDevicesAsync(
                    session.MenteeId,
                    "Session Cancelled by Mentor",
                    "Your mentor has cancelled the session. Please check the app for more details and to reschedule if needed.",
                    cancellationToken
                ));
            }
            else
            {
                BackgroundJob.Enqueue(() => _fcmService.SendPushNotificationToUserDevicesAsync(
                    session.MentorId,
                    "Session Cancelled by Mentee",
                    "Your mentee has cancelled the session. Please check the app for more details and to reschedule if needed.",
                    cancellationToken
                ));
            }

            session.Status = SessionStatus.cancelled;
            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }

        public async Task<Result> GiveFeedbackAsync(string id, FeedbackRequest request, CancellationToken cancellationToken)
        {
            var session = await _context.Sessions.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (session is null)
                return Result.Failure(SessionError.NotFoundSession);

            if (session.Status != SessionStatus.awaiting_feedback)
                return Result.Failure(SessionError.ActionNotAllawed);
            var mentor = await _context.Users.FirstAsync(x => x.Id == session.MentorId, cancellationToken);
            mentor.Rate = ((mentor.Rate * (mentor.NumberOfSession - 1)) + request.Rating) / mentor.NumberOfSession;
            await _context.SaveChangesAsync(cancellationToken);

            session.Status = SessionStatus.completed;
            session.Rating = request.Rating;

            session.Comment = request.Comment;
            BackgroundJob.Enqueue(() => _fcmService.SendPushNotificationToUserDevicesAsync(
                session.MentorId,
                "Mentee Has Rated the Session",
                "Your mentee has provided a rating for the session. Please check the app to view the feedback and rating.",
                cancellationToken
            ));
            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }

        public async Task<Result> AttendByOnePartyAsync(string id, string userId, CancellationToken cancellationToken)
        {
            var session = await _context.Sessions.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (session is null)
                return Result.Failure(SessionError.NotFoundSession);

            if (session.MentorId != userId && session.MenteeId != userId)
                return Result.Failure(SessionError.Unauthorized);

            if (session.Status != SessionStatus.accepted)
                return Result.Failure(SessionError.ActionNotAllawed);

            if (session.Status != SessionStatus.ongoing &&
                (DateTime.UtcNow.AddHours(3) < session.ScheduledTime ||
                DateTime.UtcNow.AddHours(3) > session.ScheduledTime.AddMinutes(session.WaitingTime)))
                return Result.Failure(SessionError.TimeError);



            if (session.MentorId == userId)
            {
                session.MentorJoinedAt = DateTime.UtcNow.AddHours(3);
                BackgroundJob.Enqueue(() => _fcmService.SendPushNotificationToUserDevicesAsync(
                    session.MenteeId,
                    "Mentor Joined the Meeting",
                    "Your mentor has joined the session.",
                    cancellationToken
                ));

            }

            else
            {
                session.MenteeJoinedAt = DateTime.UtcNow.AddHours(3);
                BackgroundJob.Enqueue(() => _fcmService.SendPushNotificationToUserDevicesAsync(
                    session.MentorId,
                    "Mentee Joined the Meeting",
                    "Your mentee has joined the session.",
                    cancellationToken
                ));
            }

            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
        public async Task<Result> SetSessionOutcomeAsync(string id, SessionOutcomeRequest request, CancellationToken cancellationToken)
        {
            var session = await _context.Sessions.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (session is null)
                return Result.Failure(SessionError.NotFoundSession);

            if (session.Status != SessionStatus.Disputed)
                return Result.Failure(SessionError.ActionNotAllawed);

            if (request.Status == ApprovalStatus.accepted.ToString())
            {
                session.Status = SessionStatus.expired;
                BackgroundJob.Enqueue(() => _fcmService.SendPushNotificationToUserDevicesAsync(
                    session.MentorId,
                    "Report Accepted - Refund Issued",
                    "The report filed by your mentee has been reviewed and accepted. A refund will be processed to your mentee. Please check the app for further details.",
                    cancellationToken
                ));
                BackgroundJob.Enqueue(() => _fcmService.SendPushNotificationToUserDevicesAsync(
                    session.MenteeId,
                    "Report Accepted - Refund Processed",
                    "Your report has been reviewed and accepted. A refund will be processed to you shortly. Please check the app for further details.",
                    cancellationToken
                ));
            }
            else
            {
                session.Status = SessionStatus.completed;
                BackgroundJob.Enqueue(() => _fcmService.SendPushNotificationToUserDevicesAsync(
                   session.MentorId,
                   "Report Rejected - Payment Confirmed",
                   "The report filed by your mentee has been reviewed and rejected. The payment will remain with you. Please check the app for further details.",
                   cancellationToken
               ));
                BackgroundJob.Enqueue(() => _fcmService.SendPushNotificationToUserDevicesAsync(
                    session.MenteeId,
                    "Report Rejected - No Refund Issued",
                    "The report you filed has been reviewed and rejected. No refund will be processed. Please check the app for further details.",
                    cancellationToken
                ));

            }
            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }

        public async Task<Result> SubmitSessionReportAsync(string id, CancellationToken cancellationToken)
        {
            var session = await _context.Sessions.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (session is null)
                return Result.Failure(SessionError.NotFoundSession);

            if (session.Status != SessionStatus.awaiting_feedback ||
                DateTime.UtcNow.AddHours(3) > session.ScheduledTime.AddMinutes(session.DurationMinutes).AddHours(12))
                return Result.Failure(SessionError.ActionNotAllawed);


            session.Status = SessionStatus.Disputed;
            BackgroundJob.Enqueue(() => _fcmService.SendPushNotificationToUserDevicesAsync(
                session.MentorId,
                "Mentee Reported an Issue",
                "Your mentee has filed a report regarding the session. The admin will review the issue and get in touch with you for further details.",
                cancellationToken
            ));
            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }

        public async Task CancelUnconfirmedSessionsAsync()
        {
            var now = DateTime.UtcNow.AddHours(3);

            var sessionsToCancel = await _context.Sessions
                .Where(s =>
                    (s.Status == SessionStatus.pending ||
                     s.Status == SessionStatus.awaiting_payment ||
                     s.Status == SessionStatus.awaiting_mentee_confirmation) &&
                    s.ScheduledTime < now)
                .ToListAsync();

            foreach (var session in sessionsToCancel)
            {
                session.Status = SessionStatus.cancelled;
            }

            await _context.SaveChangesAsync();
        }
        public async Task<bool> IsSessionTimeConflicting(string mentorId, DateTime scheduledTime, int durationMinutes, CancellationToken cancellationToken)
        {
            var availableStatus = new[]
            {
                SessionStatus.pending,
                SessionStatus.awaiting_mentee_confirmation,
                SessionStatus.awaiting_payment,
                SessionStatus.accepted
            };

            return await _context.Sessions
                .Where(x => x.MentorId == mentorId && availableStatus.Contains(x.Status))
                .AnyAsync(x =>
                    scheduledTime < x.ScheduledTime.AddMinutes(x.DurationMinutes + 10) &&
                    scheduledTime.AddMinutes(durationMinutes) > x.ScheduledTime,
                    cancellationToken);
        }
        public async Task<bool> IsSessionTimeConflictingForUpdate(string id, string mentorId, DateTime scheduledTime, int durationMinutes, CancellationToken cancellationToken)
        {
            var availableStatus = new[]
            {
                SessionStatus.pending,
                SessionStatus.awaiting_mentee_confirmation,
                SessionStatus.awaiting_payment,
                SessionStatus.accepted
            };

            return await _context.Sessions
                .Where(x => x.MentorId == mentorId && availableStatus.Contains(x.Status))
                .AnyAsync(x =>
                    scheduledTime < x.ScheduledTime.AddMinutes(x.DurationMinutes + 10) &&
                    scheduledTime.AddMinutes(durationMinutes) > x.ScheduledTime &&
                    x.Id != id,
                    cancellationToken);
        }
        public async Task CheckSessionExpiration(Session session)
        {
            if (session.MenteeJoinedAt.HasValue && session.MentorJoinedAt.HasValue)
            {
                session.Status = SessionStatus.ongoing;
                var time = session.ScheduledTime.AddMinutes(session.DurationMinutes);
                BackgroundJob.Schedule(() => CompleteSession(session), time);
            }
            else if (session.MenteeJoinedAt.HasValue)
                session.Status = SessionStatus.MenteeAttendedOnly;
            else if (session.MentorJoinedAt.HasValue)
                session.Status = SessionStatus.MentorAttendedOnly;
            else
                session.Status = SessionStatus.expired;
            await _context.SaveChangesAsync();
        }
        public async Task CompleteSession(Session session)
        {
            if (session.Status == SessionStatus.ongoing)
            {
                var user = _context.Users
                    .Where(x => x.Id == session.MentorId)
                    .First();
                user.NumberOfSession += 1;
                _context.SaveChanges();
                session.Status = SessionStatus.awaiting_feedback;
                session.UpdatedAt = DateTime.UtcNow.AddHours(3);
                await _context.SaveChangesAsync();
            }
        }
        public async Task SendSessionConfirmed(Session session, CancellationToken cancellationToken = default)
        {
            var mentorName = await _context.Users
                .Where(x => x.Id == session.MentorId)
                .Select(x => x.Name)
                .FirstAsync();
            var menteeDitails = await _context.Users
                .Where(x => x.Id == session.MenteeId)
                .Select(x => new { x.Name, x.Email })
                .FirstAsync();
            var emailBody = EmailBodyBuilder.GenerateEmailBody
                (
                   "SessionConfirmed",
                    new Dictionary<string, string>
                    {
                        { "{{name}}", menteeDitails.Name },
                        { "{{mentorName}}", mentorName },
                        { "{{sessionDate}}", session.ScheduledTime.ToString("dd/MM/yyyy") },
                        { "{{sessionTime}}", session.ScheduledTime.ToString("hh:mm tt") },
                        { "{{sessionPrice}}", session.Price.ToString()! }
                    }
                );
            BackgroundJob.Enqueue(() => _emailSender.SendEmailAsync(menteeDitails.Email!, "Mentorea: Session Confirmed", emailBody));
        }
        public async Task SendNotification(Session session, CancellationToken cancellationToken = default)
        {
            var menteeName = await _context.Users
                .Where(x => x.Id == session.MenteeId)
                .Select(x => x.Name)
                .FirstAsync(cancellationToken);
            var mentorName = await _context.Users
                .Where(x => x.Id == session.MentorId)
                .Select(x =>x.Name)
                .FirstAsync(cancellationToken);
            BackgroundJob.Enqueue(() => _fcmService.SendPushNotificationToUserDevicesAsync(
                session.MentorId,
                "Payment Confirmed - Session Booked",
               "Your mentee has completed the payment. The session is now confirmed. Check the app for details.",
                cancellationToken
            ));
            BackgroundJob.Enqueue(() => _fcmService.SendPushNotificationToUserDevicesAsync(
                session.MenteeId,
                "Payment Successful - Session Confirmed",
               "Your payment was successful and your session has been confirmed. Check the app for details.",
                cancellationToken
            ));
            //for reminder
            // 12 hours before the session
            BackgroundJob.Schedule(() => _fcmService.SendPushNotificationToUserDevicesAsync(
                session.MentorId,
                $"Reminder: You have a session tomorrow at {session.ScheduledTime.ToString("hh:mm tt")}",
                $"Just a reminder, your session with {menteeName} is in 12 hours at {session.ScheduledTime.ToString("hh:mm tt")}. Please be prepared and ready!",
                cancellationToken
            ), session.ScheduledTime.AddHours(-12));

            BackgroundJob.Schedule(() => _fcmService.SendPushNotificationToUserDevicesAsync(
                session.MenteeId,
                $"Reminder: Your session is tomorrow at {session.ScheduledTime.ToString("hh:mm tt")}",
                $"Just a reminder, your session with {mentorName} is in 12 hours at {session.ScheduledTime.ToString("hh:mm tt")}. Please be prepared and ready!",
                cancellationToken
            ), session.ScheduledTime.AddHours(-12));

            // 1 hour before the session
            BackgroundJob.Schedule(() => _fcmService.SendPushNotificationToUserDevicesAsync(
                session.MentorId,
                $"Reminder: Your session is starting in 1 hour at {session.ScheduledTime.ToString("hh:mm tt")}",
                $"Just a reminder, your session with {menteeName} is starting in 1 hour at {session.ScheduledTime.ToString("hh:mm tt")}. Please be prepared!",
                cancellationToken
            ), session.ScheduledTime.AddHours(-1));

            BackgroundJob.Schedule(() => _fcmService.SendPushNotificationToUserDevicesAsync(
                session.MenteeId,
                $"Reminder: Your session is starting in 1 hour at {session.ScheduledTime.ToString("hh:mm tt")}",
                $"Just a reminder, your session with {mentorName} is starting in 1 hour at {session.ScheduledTime.ToString("hh:mm tt")}. Please be prepared!",
                cancellationToken
            ), session.ScheduledTime.AddHours(-1));

            // 5 minutes before the session
            BackgroundJob.Schedule(() => _fcmService.SendPushNotificationToUserDevicesAsync(
                session.MentorId,
                $"Reminder: Your session is starting in 5 minutes at {session.ScheduledTime.ToString("hh:mm tt")}",
                $"Just a reminder, your session with {menteeName} is starting in 5 minutes at {session.ScheduledTime.ToString("hh:mm tt")}. Please be ready!",
                cancellationToken
            ), session.ScheduledTime.AddMinutes(-5));

            BackgroundJob.Schedule(() => _fcmService.SendPushNotificationToUserDevicesAsync(
                session.MenteeId,
                $"Reminder: Your session is starting in 5 minutes at {session.ScheduledTime.ToString("hh:mm tt")}",
                $"Just a reminder, your session with {mentorName} is starting in 5 minutes at {session.ScheduledTime.ToString("hh:mm tt")}. Please be ready!",
                cancellationToken
            ), session.ScheduledTime.AddMinutes(-5));
        }
    }
}
