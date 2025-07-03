
using Mentorea.Contracts.Common;
using Mentorea.Contracts.Users;

namespace Mentorea.Services
{
    public interface IAuthService
    {
        
        Task<Result<AuthResponse>> GetTokenAsync (LoginRequest request,CancellationToken cancellationToken=default);
        Task<Result<AuthResponse>> GetRefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken);
        Task<Result> RevokeRefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken);
        Task<Result> MentorRegisterAsync(MentorRegisterRequest request, CancellationToken cancellationToken);
        Task<Result> MenteeRegisterAsync(MenteeRegisterRequest request,  CancellationToken cancellationToken);
        Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request, CancellationToken cancellationToken);
        Task<Result> ResendConfirmEmailAsync(ResendConfirmEmailRequest request, CancellationToken cancellationToken);
        Task<Result> SendResetPasswordCodeAsync(ForgetPasswordRequest request);
        Task<Result> RestPasswordAsync(ResetPasswordRequest request);
    }
}
