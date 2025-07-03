

using Mentorea.Contracts.Roles;

namespace Mentorea.Services
{
    public interface IRoleService
    {
        Task<IEnumerable<RoleResponse>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<Result<RoleDetailResponse>> GetAsync(string id);
        Task<Result<RoleDetailResponse>> AddAsync(RoleRequest request);
        Task<Result> UpdateAsync(string id, RoleRequest request);
        Task<Result> DeleteAsync(string id);
    }
}
