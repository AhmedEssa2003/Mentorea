using Mentorea.Contracts.Card;

namespace Mentorea.Services
{
    public interface ICardService
    {
        Task<Result<string>> CreateAsync(string userId, CardRequest request, CancellationToken cancellationToken);
        Task<Result<string>> GetAsync(string userId, CancellationToken cancellationToken);
        Task<Result> UpdateAsync(string userId, CardRequest request, CancellationToken cancellationToken);
        Task<Result> DeleteAsync(string userId, CancellationToken cancellationToken);
    }
}
