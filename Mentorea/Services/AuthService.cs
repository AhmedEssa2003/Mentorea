
using Hangfire;
using Mentorea.Abstractions.Enums;
using Mentorea.Authentication;
using Mentorea.Errors;
using Mentorea.Helpers;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Security.Cryptography;
using System.Linq.Dynamic.Core;

namespace Mentorea.Services
{
    public class AuthService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ILogger<AuthService> logger,
        IJwtProvider jwtProvider,
        IEmailSender emailSender,
        IHttpContextAccessor httpContextAccessor,
        IFileService fileService,
        IDistributedCacheService cacheService,
        MentoreaDbContext context
        ) : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
        private readonly ILogger<AuthService> _logger = logger;
        private readonly IJwtProvider _jwtProvider = jwtProvider;
        private readonly IEmailSender _emailSender = emailSender;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly IFileService _fileService = fileService;
        private readonly IDistributedCacheService _cacheService = cacheService;
        private readonly MentoreaDbContext _context = context;
        private readonly int _refreshTokenExpiryDays = 15;

        
        
        public async Task<Result<AuthResponse>> GetTokenAsync(LoginRequest request, CancellationToken cancellationToken)
        {
            if (await _userManager.FindByEmailAsync(request.Email) is not { } user)
                return Result.Failure<AuthResponse>(UserError.InvalidCredential);
            if(user.IsDisabled)
                return Result.Failure<AuthResponse>(UserError.DisaledUser);
            var result = await _signInManager.PasswordSignInAsync(user, request.Password, false, true);
            if (result.Succeeded)
            {
                
                var response = await GenrateToenAndRefreshTokenAsync(user);
                return Result.Success(response);
            }
            var error = result.IsNotAllowed
                ?UserError.EmailNotConfirmed
                :result.IsLockedOut
                ?UserError.LockedUser
                :UserError.InvalidCredential;

            return Result.Failure<AuthResponse>(error);
        }
        public async Task<Result<AuthResponse>> GetRefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken)
        {
            if (_jwtProvider.ValidateToken(request.Token) is not { } userId)
                return Result.Failure<AuthResponse>(UserError.InvalidToken);
            if (await _userManager.FindByIdAsync(userId) is not { } user)
                return Result.Failure<AuthResponse>(UserError.NotFoundUser);
            if (user.IsDisabled)
                return Result.Failure<AuthResponse>(UserError.DisaledUser);
            if (user.LockoutEnd>DateTime.UtcNow.AddHours(3))
                return Result.Failure<AuthResponse>(UserError.LockedUser);
            if (user.RefreshTokens.SingleOrDefault(x=>!x.IsExpired&&x.Token== request.RefreshToken) is not { } refreshFromDB)
                return Result.Failure<AuthResponse>(UserError.InvalidCode);
            refreshFromDB.RevokedOn = DateTime.UtcNow.AddHours(3);
            var response = await GenrateToenAndRefreshTokenAsync(user);
            return Result.Success(response);
        }
        public async Task<Result> RevokeRefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken)
        {
            if (_jwtProvider.ValidateToken(request.Token) is not { } userId)
                return Result.Failure(UserError.InvalidToken);
            if (await _userManager.FindByIdAsync(userId) is not { } user)
                return Result.Failure(UserError.NotFoundUser);
            if (user.RefreshTokens.SingleOrDefault(x => !x.IsExpired && x.Token == request.RefreshToken) is not { } refreshFromDB)
                return Result.Failure(UserError.InvalidCode);
            refreshFromDB.RevokedOn = DateTime.UtcNow.AddHours(3);
            await _userManager.UpdateAsync(user);
            return Result.Success();
        }
        public async Task<Result> MentorRegisterAsync(MentorRegisterRequest request, CancellationToken cancellationToken)
        {
            var emailIsExists = await _userManager.Users.AnyAsync(x => x.Email == request.Email,cancellationToken);
            if (emailIsExists)
                return Result.Failure(UserError.DuplicateEmail);
            if (!await _context.Fields.AnyAsync(x=>x.Id==request.FieldId,cancellationToken))
                return Result.Failure(FieldError.NotFound);
            var user = request.Adapt<ApplicationUser>();
            if (request.Image is not null)
            {
                var createdImageName = await _fileService.SaveImageAsync(request.Image, "Users");
                var basUrl = _fileService.GetBaseUrl(_httpContextAccessor);
                user.PathPhoto = $"{basUrl}Images/Users/{createdImageName}";
            }
            var result = await _userManager.CreateAsync(user,request.Password);
            if (result.Succeeded)
            {
                var otp = await GenrateOTPEmailCode(user);
                
                await _userManager.AddToRoleAsync(user,DefaultRole.Mentor);
                await SendConfirmationEmail(user, otp);
                return Result.Success();
            }
            var error = result.Errors.First();
            return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
        }
        public async Task<Result> MenteeRegisterAsync(MenteeRegisterRequest request, CancellationToken cancellationToken)
        {
            var emailIsExists = await _userManager.Users.AnyAsync(x => x.Email == request.Email, cancellationToken);
            if (emailIsExists)
                return Result.Failure<AuthResponse>(UserError.DuplicateEmail);

            var user = request.Adapt<ApplicationUser>();

            if (request.Image is not null)
            {
                var createdImageName = await _fileService.SaveImageAsync(request.Image, "Users");
                var baseUrl = _fileService.GetBaseUrl(_httpContextAccessor);
                user.PathPhoto = $"{baseUrl}Images/Users/{createdImageName}";
            }

            var existingFieldIds = await _context.Fields
                .Where(f => request.FieldInterests.Contains(f.Id))
                .Select(f => f.Id)
                .ToListAsync(cancellationToken);

            var invalidFields = request.FieldInterests.Except(existingFieldIds).ToList();
            if (invalidFields.Any())
                return Result.Failure(FieldError.NotFound);

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                var error = result.Errors.First();
                return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
            }

            foreach (var item in request.FieldInterests)
            {
                _context.MenteeFields.Add(new MenteeFieldInterests
                {
                    FieldId = item,
                    MenteeId = user.Id
                });
            }

            await _context.SaveChangesAsync(cancellationToken);

            var otp = await GenrateOTPEmailCode(user);

            await _userManager.AddToRoleAsync(user, DefaultRole.Mentee);

            await SendConfirmationEmail(user, otp);

            return Result.Success();

        }
        public async Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request, CancellationToken cancellationToken)
        {
            if (await _userManager.FindByEmailAsync(request.Email) is not { } user)
                return Result.Failure(UserError.NotFoundUser);

            if (user.EmailConfirmed)
                return Result.Failure(UserError.DuplicatedConfirmation);

            
            int otp = int.Parse(request.Code);

            if (user.OTPs.SingleOrDefault(x => !x.IsExpired() && x.Code == otp && x.Purpose == OtpPurpose.EmailConfirmation) is not { } otpFromDB)
                return Result.Failure(UserError.InvalidCode);
            if (user.NumberOfExperience is not null)
                await InvalidateMentorCacheAsync();
            user.EmailConfirmed = true;
            otpFromDB.RevokedAt = DateTime.UtcNow.AddHours(3);

            await _userManager.UpdateAsync(user);
            return Result.Success();

        }
        public async Task<Result> ResendConfirmEmailAsync(ResendConfirmEmailRequest request, CancellationToken cancellationToken)
        {
            if (await _userManager.FindByEmailAsync(request.Email) is not { } user)
                return Result.Success();
            if (user.EmailConfirmed)
                return Result.Failure(UserError.DuplicatedConfirmation);

            var otp = await GenrateOTPEmailCode(user);
            

            await SendConfirmationEmail(user, otp);

            return Result.Success();

        }
        public async Task<Result>SendResetPasswordCodeAsync(ForgetPasswordRequest request)
        {
            if (await _userManager.FindByEmailAsync(request.Email) is not { } user)
                return Result.Success();
            if (!user.EmailConfirmed)
                return Result.Failure(UserError.EmailNotConfirmed);
            var code = await GenrateOTPPassCode(user);
            
            await SendRestPasswordEmail(user, code);

            return Result.Success();
        }
        public async Task<Result> RestPasswordAsync(ResetPasswordRequest request)
        {
            if (await _userManager.FindByEmailAsync(request.Email) is not { } user)
                return Result.Failure(UserError.InvalidCode);
            if (!user.EmailConfirmed)
                return Result.Failure(UserError.InvalidCode);
            var otp = int.Parse(request.Code);
            if (user.OTPs.SingleOrDefault(x => !x.IsExpired() && x.Code == otp && x.Purpose == OtpPurpose.PasswordReset) is not { } otpFromDB)
                return Result.Failure(UserError.InvalidCode);

            var passwordHasher = new PasswordHasher<ApplicationUser>();
            var hashedPassword = passwordHasher.HashPassword(user, request.NewPassword);
            user.PasswordHash = hashedPassword;

            otpFromDB.RevokedAt = DateTime.UtcNow.AddHours(3);
            await _userManager.UpdateAsync(user);
            return Result.Success();
        }
        private async Task SendConfirmationEmail(ApplicationUser user, int code)
        {
            var Origin = _httpContextAccessor.HttpContext?.Request.Headers.Origin;

            var EmailBody = EmailBodyBuilder.GenerateEmailBody("EmailConfirmation", new Dictionary<string, string>
                {
                {"{{name}}",user.Name },
                {"{{code}}", code.ToString() }
            });

            BackgroundJob.Enqueue(() => _emailSender.SendEmailAsync(user.Email!, "Mentorea", EmailBody));
            await Task.CompletedTask;
        }
        private async Task SendRestPasswordEmail(ApplicationUser user, int code)
        {

            var EmailBody = EmailBodyBuilder.GenerateEmailBody("ForgetPassword",
                new Dictionary<string, string>
                {
                    {"{{UserName}}",user.Name },
                    {"{{ResetCode}}", code.ToString() }
            });

            BackgroundJob.Enqueue(() => _emailSender.SendEmailAsync(user.Email!, "Mentorea:Change password", EmailBody));
            await Task.CompletedTask;
        }
        private async Task<string> GenrateRefreshToken (ApplicationUser user)
        {
            var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            user.RefreshTokens.Add(new RefreshToken { 
                Token = refreshToken,
                ExpiresOn = DateTime.UtcNow.AddHours(3).AddDays(_refreshTokenExpiryDays) 
            });
            await _userManager.UpdateAsync(user);
            return refreshToken;
            
        }
        private async Task<int>GenrateOTPEmailCode(ApplicationUser user)
        {
            var otps = user.OTPs.Where(x => !x.IsExpired() && x.Purpose == OtpPurpose.EmailConfirmation);
            foreach (var item in otps)
            {
                item.RevokedAt = DateTime.UtcNow.AddHours(3);
            }
            var otp = RandomNumberGenerator.GetInt32(100000, 999999+1);
            user.OTPs.Add(new OTP
            {
                Code = otp,
                Purpose = OtpPurpose.EmailConfirmation,
                ExpiredAt = DateTime.UtcNow.AddHours(3).AddMinutes(3)
            });
            await _userManager.UpdateAsync(user);
            return otp;
        }
        private async Task<int> GenrateOTPPassCode(ApplicationUser user)
        {
            var otps = user.OTPs.Where(x => !x.IsExpired() && x.Purpose == OtpPurpose.PasswordReset);
            foreach (var item in otps)
            {
                item.RevokedAt = DateTime.UtcNow.AddHours(3);
            }
            var otp = RandomNumberGenerator.GetInt32(100000, 999999 + 1);
            user.OTPs.Add(new OTP
            {
                Code = otp,
                ExpiredAt = DateTime.UtcNow.AddHours(3).AddMinutes(3),
                Purpose = OtpPurpose.PasswordReset
            });
            await _userManager.UpdateAsync(user);
            return otp;
        }
        private async Task<AuthResponse> GenrateToenAndRefreshTokenAsync(ApplicationUser user)
        {
            var userRole = await _userManager.GetRolesAsync(user);
            var UserRoleName = userRole.First();
            var (token, expirsIn) = _jwtProvider.GenrateToken(user, UserRoleName);
            var refreshToken = await GenrateRefreshToken(user);
            var response = new AuthResponse(
                            user.Id,
                            user.Email!,
                            user.Name,
                            token,
                            expirsIn,
                            refreshToken,
                            DateOnly.FromDateTime(DateTime.UtcNow.AddHours(3).AddDays(_refreshTokenExpiryDays))
            );

            return response;

        }
        private async Task InvalidateMentorCacheAsync()
        {
            for (int i = 1; i <= 3; i++)
            {
                string key = $"mentors:search=all:sort=Rate:direction=DESC:page={i}:size=10";
                await _cacheService.RemoveAsync(key);
            }
        }

    }
}
