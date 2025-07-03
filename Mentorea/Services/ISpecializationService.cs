using Mentorea.Contracts.Specializations;

namespace Mentorea.Services
{
    public interface ISpecializationService
    {
        Task<Result<List<SpecializationResponse>>> GetAllAsync(CancellationToken cancellationToken);
        Task<Result<SpecializationResponse>> GetByIdAsync(string id,CancellationToken cancellationToken);
        Task<Result<SpecializationResponse>> CreateAsync(SpecializationRequest request,CancellationToken cancellation);
        Task<Result> UpdateAsync(string id, SpecializationRequest request,CancellationToken cancellation);
        Task<Result> DeleteAsync(string id, CancellationToken cancellation);
    }

}
