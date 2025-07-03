using Mentorea.Contracts.Users;
using Mentorea.Errors;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Mentorea.Services
{
    public interface IUserService
    {
        Task<Result<MenteeProfileResponse>> GetMenteeProfileAsync(string userId);
        Task<Result<MentorProfileResponse>> GetMentorProfileAsync(string userId);
        Task<Result<AdminProfileResponse>> GetAdminProfileAsync(string userId);
        Task<Result> UpdateMentorProfileAsync(string userId, UpdateMentorProfileRequest request);
        Task<Result> UpdateMenteeProfileAsync(string userId, UpdateMenteeProfileRequest request);
        Task<Result> UpdateAdminProfileAsync(string userId, UpdateAdminProfileRequest request);
        Task<Result> ChangePasswordAsync(string userId, ChangePasswordRequest request);
        Task<IEnumerable<UserResponse>> GetAllAsync(CancellationToken cancellationToken);
        Task<Result<UserResponse>> AddAsync(CreateUserRequest request, CancellationToken cancellationToken);
        Task<Result<UserResponse>> GetAsync(string Id);
        Task<Result> UnLockAsync(string Id, CancellationToken cancellationToken);
        Task<Result> ToggleStatusAsync(string Id, CancellationToken cancellationToken);
        Task<Result> UpdateImageAsync(string userId, ImageRequest request);
    }
}
