using Mentorea.Abstractions.Enums;
using Mentorea.Contracts.Dashboard;
using Mentorea.Contracts.Session;

namespace Mentorea.Services
{
    public class DashboardService(MentoreaDbContext context): IDashboardService
    {
        private readonly MentoreaDbContext _context = context;

        public async Task<int>CountOfMentorAsync(CancellationToken cancellationToken)
        { 
            return await _context.Users.Where(x => x.NumberOfExperience.HasValue).CountAsync(cancellationToken);
        }
        public async Task<int>CountOfMenteeAsync(CancellationToken cancellationToken)
        {
            return await (from u in  _context.Users
             join ur in _context.UserRoles on u.Id equals ur.UserId
             join r in _context.Roles on ur.RoleId equals r.Id
             where r.Name == DefaultRole.Mentee
             select u.Id).CountAsync(cancellationToken);
        }
        public async Task<int>CountOfPostAsync(CancellationToken cancellationToken)
        {
            return await _context.Posts.CountAsync(cancellationToken);
        }
        public async Task<int>CountOfSessionAsync(CancellationToken cancellationToken)
        {
            return await _context.Sessions.CountAsync(cancellationToken);
        }
        public async Task<Result<List<DashboardSessionResponse>>> GetPaymentRecipientAsync(CancellationToken cancellationToken)
        {
            List<DashboardSessionResponse> response = new List<DashboardSessionResponse>();
            var sessions = await _context.PymentSessions.ToListAsync(cancellationToken);
            foreach (var sessionId in sessions)
            {
                var session = await _context.Sessions
                    .Where(x=>x.Id == sessionId.SessionId)
                    .SingleAsync(cancellationToken);
                if ((session.Status == SessionStatus.awaiting_feedback && DateTime.UtcNow.AddHours(3).AddHours(-12) >= session.UpdatedAt) || session.Status == SessionStatus.MentorAttendedOnly || session.Status == SessionStatus.completed)
                {
                    var user = await _context.Users
                        .Where(x => x.Id == session.MentorId)
                        .Select(x => new DashboardSessionResponse(
                            x.Id,
                            x.Id,
                            x.Name,
                            x.Card.CardId,
                            session.Status.ToString(),
                            session.Price))
                        .SingleAsync(cancellationToken);
                    response.Add(user);
                }
                if (session.Status == SessionStatus.expired || session.Status == SessionStatus.MenteeAttendedOnly)
                {
                    var user = await _context.Users
                        .Where(x => x.Id == session.MenteeId)
                        .Select(x => new DashboardSessionResponse(
                            x.Id,
                            x.Id,
                            x.Name,
                            x.Card.CardId,
                            session.Status.ToString(),
                            session.Price))
                        .SingleAsync(cancellationToken);
                    response.Add(user);
                }

                
            }
            return Result.Success(response);
        }

        public async Task<Result> DeleteFromListAsync(string SessionId, CancellationToken cancellationToken)
        {
            await _context.PymentSessions
                .Where(x => x.SessionId == SessionId)
                .ExecuteDeleteAsync(cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
