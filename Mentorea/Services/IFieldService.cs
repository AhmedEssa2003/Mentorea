using Mentorea.Contracts.Fields;

namespace Mentorea.Services
{
    public interface IFieldService
    {
        Task<Result<List<FieldResponse>>> GetAll(CancellationToken cancellationToken);
        Task<Result<SingleFieldResponse>> GetAsync(string Id, CancellationToken cancellationToken);
        Task<Result<SingleFieldResponse>> CreateAsync(FieldRequest request, CancellationToken cancellationToken);
        Task<Result> UpdateAsync(string Id, FieldRequest request, CancellationToken cancellationToken);
        Task<Result> DeleteAsync(string Id, CancellationToken cancellationToken);
    }
}
