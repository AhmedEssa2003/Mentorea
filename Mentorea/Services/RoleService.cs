
using Mentorea.Contracts.Roles;
using SurveyBasket.Errors;

namespace Mentorea.Services
{
    public class RoleService(RoleManager<ApplicationRole> roleManager, MentoreaDbContext context) : IRoleService
    {
        private readonly RoleManager<ApplicationRole> _roleManager = roleManager;
        private readonly MentoreaDbContext _context = context;

        public async Task<IEnumerable<RoleResponse>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _roleManager.Roles
                .ProjectToType<RoleResponse>()
                .ToListAsync(cancellationToken);
        }
        public async Task<Result<RoleDetailResponse>> GetAsync(string id)
        {

            if (await _roleManager.FindByIdAsync(id) is not { } Role)
                return Result.Failure<RoleDetailResponse>(RoleError.RoleNotFound);
            var response = new RoleDetailResponse(Role.Id, Role.Name!);
            return Result.Success(response);

        }
        public async Task<Result<RoleDetailResponse>> AddAsync(RoleRequest request)
        {

            var isExists = await _roleManager.RoleExistsAsync(request.Name);
            if (isExists)
                return Result.Failure<RoleDetailResponse>(RoleError.DuplicateRole);

            var role = new ApplicationRole
            {
                Name = request.Name,
                ConcurrencyStamp = Guid.NewGuid().ToString()
            };
            var result = await _roleManager.CreateAsync(role);
            if (!result.Succeeded)
            {
                var error = result.Errors.First();
                return Result.Failure<RoleDetailResponse>(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
            }
            
            var response = new RoleDetailResponse(role.Id, role.Name);
            return Result.Success(response);


        }
        public async Task<Result> DeleteAsync(string id)
        {
            if (await _roleManager.FindByIdAsync(id) is not { } role)
                return Result.Failure<RoleDetailResponse>(RoleError.RoleNotFound);

            var result = await _roleManager.DeleteAsync(role);
            if (!result.Succeeded)
            {
                var error = result.Errors.First();
                return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
            }
            return Result.Success();
        }
        public async Task<Result> UpdateAsync(string id, RoleRequest request)
        {

            var isexists = await _roleManager.Roles.AnyAsync(x => x.Name == request.Name && x.Id != id);
            if (isexists)
                return Result.Failure(RoleError.DuplicateRole);

            if (await _roleManager.FindByIdAsync(id) is not { } role)
                return Result.Failure<RoleDetailResponse>(RoleError.RoleNotFound);

            role.Name = request.Name;
            var result = await _roleManager.UpdateAsync(role);
            if (!result.Succeeded)
            {
                var error = result.Errors.First();
                return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
            }
            
            return Result.Success();


        }

        
    }
}
