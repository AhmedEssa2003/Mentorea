using Mentorea.Contracts.MentorAvailability;
using Mentorea.Errors;

namespace Mentorea.Services
{
    public class MentorAvailabilityService(MentoreaDbContext context):IMentorAvailabilityService
    {
        private readonly MentoreaDbContext _context = context;

        public async Task<Result<List<MentorAvailabilityResponse>>> GetAllAsync (string mentorId ,CancellationToken cancellationToken)
        {
            var response = await _context.MentorAvailability
                .Where(x => x.MentorId == mentorId && DateTime.UtcNow.AddHours(3) < x.EndTime)
                .Select(x => new MentorAvailabilityResponse
                (
                    x.Id,
                    x.MentorId,
                    x.StartTime.ToString("dd/MM/yyyy"),
                    x.StartTime.ToString("HH:mm"),
                    x.EndTime.ToString("HH:mm")
                ))
                .ToListAsync(cancellationToken);
            return Result.Success(response);
        }
        public async Task<Result<MentorAvailabilityResponse>> GetAsync(string id, CancellationToken cancellationToken)
        {
            var response = await _context.MentorAvailability
                .Where(x => x.Id == id)
                .Select(x => new MentorAvailabilityResponse
                (
                    x.Id,
                    x.MentorId,
                   x.StartTime.ToString("dd/MM/yyyy"),
                    x.StartTime.ToString("HH:mm"),
                    x.EndTime.ToString("HH:mm")

                ))
                .SingleOrDefaultAsync(cancellationToken);
            if (response is null)
                return Result.Failure<MentorAvailabilityResponse>(MentorAvailabilityError.NotFound);
            return Result.Success(response);
        }
        public async Task<Result<MentorAvailabilityResponse>> CreateAsync(string mentorId ,MentorAvailabilityRequest request, CancellationToken cancellationToken)
        {
            if (!await _context.Users.AnyAsync(x => x.Id == mentorId && !x.IsDisabled, cancellationToken))
                return Result.Failure<MentorAvailabilityResponse>(MentorAvailabilityError.MentorNotFound);
            if (request.StartDateTime is null || request.EndDateTime is null)
                return Result.Failure<MentorAvailabilityResponse>(MentorAvailabilityError.InvalidTime);
            if (await _context.MentorAvailability
                 .Where(x => x.MentorId == mentorId)
                 .AnyAsync(x =>
                     request.StartDateTime < x.EndTime &&
                     request.EndDateTime > x.StartTime,
                     cancellationToken))
            {
                return Result.Failure<MentorAvailabilityResponse>(MentorAvailabilityError.OverlappingTime);
            }

            var mentorAvailability = new MentorAvailability {
                EndTime = request.EndDateTime!.Value,
                StartTime = request.StartDateTime!.Value,
                MentorId = mentorId,
            };
            await _context.MentorAvailability.AddAsync(mentorAvailability, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            var response = new MentorAvailabilityResponse(mentorAvailability.Id,
                    mentorAvailability.MentorId,
                    mentorAvailability.StartTime.ToString("dd/MM/yyyy"),
                    mentorAvailability.StartTime.ToString("HH:mm"),
                    mentorAvailability.EndTime.ToString("HH:mm"));
            return Result.Success(response);
        }
        public async Task<Result> UpdateAsync (string id, MentorAvailabilityRequest request ,CancellationToken cancellationToken)
        {
            var mentorAvailability = await _context.MentorAvailability
                .Where(x => x.Id == id)
                .SingleOrDefaultAsync(cancellationToken);
            if (mentorAvailability is null)
                return Result.Failure(MentorAvailabilityError.NotFound);
            if (await _context.MentorAvailability
                .Where(x => x.MentorId == mentorAvailability.MentorId)
                .AnyAsync(x =>
                request.StartDateTime < x.EndTime && request.EndDateTime > x.StartTime && x.Id != id, cancellationToken))
                return Result.Failure(MentorAvailabilityError.OverlappingTime);
            mentorAvailability.StartTime = request.StartDateTime!.Value;
            mentorAvailability.EndTime = request.EndDateTime!.Value;
            mentorAvailability.UpdatedAt = DateTime.UtcNow.AddHours(3);
            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
        public async Task<Result> DeleteAsync(string id, CancellationToken cancellationToken)
        {
            var mentorAvailability = await _context.MentorAvailability
                .Where(x => x.Id == id)
                .SingleOrDefaultAsync(cancellationToken);
            if (mentorAvailability is null)
                return Result.Failure(MentorAvailabilityError.NotFound);
            _context.MentorAvailability.Remove(mentorAvailability);
            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
