
using Mentorea.Contracts.Users;

using Mentorea.Errors;

using Mentorea.Services;

namespace Mentorea.Services
{
    public class UserService(
        UserManager<ApplicationUser> userManager,
        MentoreaDbContext context,
        IFileService fileService,
        RoleManager<ApplicationRole> roleManager,
        IRoleService roleService,
        IHttpContextAccessor httpContextAccessor,
        IDistributedCacheService cacheService

        ) : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly MentoreaDbContext _context = context;
        private readonly IFileService _fileService = fileService;
        private readonly RoleManager<ApplicationRole> _roleManager = roleManager;
        private readonly IRoleService _roleService = roleService;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly IDistributedCacheService _cacheService = cacheService;

        public async Task<Result<MentorProfileResponse>> GetMentorProfileAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            string? fieldName = await _context.Fields.Where(x => x.Id == user!.FieldId).Select(x => x.FieldName).SingleOrDefaultAsync();
            var countComment = await _context.Sessions
                .Where(x=>x.MentorId == userId && (!string.IsNullOrEmpty(x.Comment)))
                .CountAsync();
            var response = new MentorProfileResponse(
                user!.Id,
                user!.Name,
                user.Email!,
                user!.PathPhoto!,
                user.PirthDate,
                user.Location,
                user.Rate,
                user.PriceOfSession,
                user.NumberOfSession,
                user.NumberOfExperience,
                countComment,
                user.About,
                fieldName
            );


            return Result.Success(response);
        }
        public async Task<Result<MenteeProfileResponse>> GetMenteeProfileAsync(string userId)
        {
            var response = await _userManager.Users.Where(x => x.Id == userId).ProjectToType<MenteeProfileResponse>().SingleAsync();

            return Result.Success(response);
        }
        public async Task<Result<AdminProfileResponse>> GetAdminProfileAsync(string userId)
        {
            var response = await _userManager.Users.Where(x => x.Id == userId).ProjectToType<AdminProfileResponse>().SingleAsync();

            return Result.Success(response);
        }
        public async Task<Result<UserResponse>> AddAsync(CreateUserRequest request, CancellationToken cancellationToken)
        {
            if (await _userManager.Users.AnyAsync(x => x.Email == request.Email, cancellationToken))
                return Result.Failure<UserResponse>(UserError.DuplicateEmail);
            var user = request.Adapt<ApplicationUser>();
            user.EmailConfirmed = true;
            var AllowedRoles = await _roleService.GetAllAsync(cancellationToken: cancellationToken);
            if (AllowedRoles.Select(x=>x.Name).Contains(request.Role))
                return Result.Failure<UserResponse>(UserError.InvalidRole);
            var result = await _userManager.CreateAsync(user, request.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user,request.Role);
                var response = new UserResponse(user.Id, user.Name, user.Email!, request.Role);
                return Result.Success(response);
            }
            var error = result.Errors.First();
            return Result.Failure<UserResponse>(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
        }
        public async Task<Result> UpdateMentorProfileAsync(string userId, UpdateMentorProfileRequest request)
        {
            var user = await _userManager.FindByIdAsync(userId);
            user = request.Adapt(user);
            var result = await _userManager.UpdateAsync(user!);
            if (result.Succeeded)
                return Result.Success();
            var error = result.Errors.First();
            return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
        }
        public async Task<Result> UpdateMenteeProfileAsync(string userId, UpdateMenteeProfileRequest request)
        {
            var user = await _userManager.FindByIdAsync(userId);
            user = request.Adapt(user);
            
            var result = await _userManager.UpdateAsync(user!);
            if (result.Succeeded)
                return Result.Success();
            var error = result.Errors.First();
            return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
        }
        public async Task<Result> UpdateAdminProfileAsync (string userId, UpdateAdminProfileRequest request)
        {
            var user = await _userManager.FindByIdAsync(userId);
            user = request.Adapt(user);
            var result = await _userManager.UpdateAsync(user!);
            if (result.Succeeded)
                return Result.Success();
            var error = result.Errors.First();
            return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
        }
        public async Task<Result> UpdateImageAsync(string userId,ImageRequest request)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user!.PathPhoto is not null)
            {
                var path = Path.GetFileName(user.PathPhoto);
                _fileService.DeleteImage(path!, "Users");
            }

            if (request.Image is not null)
            {
                var createdFileName = await _fileService.SaveImageAsync(request.Image, "Users");
                var baseUrl = _fileService.GetBaseUrl(_httpContextAccessor);
                user.PathPhoto = $"{baseUrl}Images/Users/{createdFileName}";
            }
            else
            {
                user.PathPhoto = null;
            }
            var result = await _userManager.UpdateAsync(user!);
            if (result.Succeeded)
                return Result.Success();
            var error = result.Errors.First();
            return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
        }
        public async Task<Result> ChangePasswordAsync(string userId, ChangePasswordRequest request)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var result = await _userManager.ChangePasswordAsync(user!, request.CurrentPassword, request.NewPassword);
            if (result.Succeeded)
                return Result.Success();
            var error = result.Errors.First();
            return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
        }
        public async Task<IEnumerable<UserResponse>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await (
                        from u in _context.Users
                        join ur in _context.UserRoles on u.Id equals ur.UserId
                        join r in _context.Roles on ur.RoleId equals r.Id
                        where r.Name != DefaultRole.Mentor && r.Name != DefaultRole.Mentee
                        select new UserResponse(
                            u.Id,
                            u.Name,
                            u.Email!,
                            r.Name!
                        )
                    ).ToListAsync(cancellationToken);


        }
        public async Task<Result<UserResponse>> GetAsync(string Id)
        {
            if (await _userManager.FindByIdAsync(Id) is not { } user)
                return Result.Failure<UserResponse>(UserError.NotFoundUser);
            var Roles = await _userManager.GetRolesAsync(user);
            var response = new UserResponse(user.Id,user.Name,user.Email!,Roles.First());
            return Result.Success(response);
        }
        public async Task<Result> UnLockAsync(string Id, CancellationToken cancellationToken)
        {
            if (await _userManager.FindByIdAsync(Id) is not { } user)
                return Result.Failure(UserError.NotFoundUser);
            await _userManager.SetLockoutEndDateAsync(user, null);
            return Result.Success();
        }
        public async Task<Result> ToggleStatusAsync(string Id, CancellationToken cancellationToken)
        {

            if (await _userManager.FindByIdAsync(Id) is not { } user)
                return Result.Failure(UserError.NotFoundUser);
            user.IsDisabled = !user.IsDisabled;
            if (user.NumberOfExperience is not null)
                await InvalidateMentorCacheAsync();
            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success();

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
