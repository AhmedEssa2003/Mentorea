using Mentorea.Contracts.MentorAvailability;

namespace Mentorea.Services
{
    public interface IMentorAvailabilityService
    {
        Task<Result<List<MentorAvailabilityResponse>>> GetAllAsync(string mentorId, CancellationToken cancellationToken);
        Task<Result<MentorAvailabilityResponse>> GetAsync(string id, CancellationToken cancellationToken);
        Task<Result<MentorAvailabilityResponse>> CreateAsync(string mentorId,MentorAvailabilityRequest request, CancellationToken cancellationToken);
        Task<Result> UpdateAsync(string id, MentorAvailabilityRequest request, CancellationToken cancellationToken);
        Task<Result> DeleteAsync(string id, CancellationToken cancellationToken);
    }
}
