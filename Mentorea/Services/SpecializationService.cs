using Mentorea.Contracts.Specializations;
using Mentorea.Errors;

namespace Mentorea.Services
{
    public class SpecializationService(MentoreaDbContext context) : ISpecializationService
    {
        private readonly MentoreaDbContext _context = context;

        public async Task<Result<SpecializationResponse>> CreateAsync(SpecializationRequest request, CancellationToken cancellation)
        {
            if (await _context.Specializations.AnyAsync(x => x.Name == request.Name, cancellation))
                return Result.Failure<SpecializationResponse>(SpecializationError.NameAlreadyExists);
            var specialization = new Specialization { Name = request.Name };
            await _context.Specializations.AddAsync(specialization, cancellation);
            await _context.SaveChangesAsync(cancellation);
            return Result.Success(specialization.Adapt<SpecializationResponse>());

        }

        public async Task<Result> DeleteAsync(string id, CancellationToken cancellation)
        {
            if (await _context.Specializations.FirstOrDefaultAsync(x=>x.Id == id,cancellation) is not { } specializations)
                return Result.Failure(SpecializationError.NotFound);
            try
            {
                _context.Specializations.Remove(specializations);
                await _context.SaveChangesAsync(cancellation);

            }
            catch 
            {
                return Result.Failure(SpecializationError.HasDependentSpecializations);

            }
            return Result.Success();
        }

        public async Task<Result<List<SpecializationResponse>>> GetAllAsync(CancellationToken cancellationToken)
        {

            var response = await _context.Specializations
                .ProjectToType<SpecializationResponse>()
                .ToListAsync(cancellationToken);
            return Result.Success(response);
        }

        public async Task<Result<SpecializationResponse>> GetByIdAsync(string id, CancellationToken cancellationToken)
        {
            if (await _context.Specializations.FirstOrDefaultAsync(x=>x.Id == id, cancellationToken) is not { } specializations)
                return Result.Failure<SpecializationResponse>(SpecializationError.NotFound);
            return Result.Success(specializations.Adapt<SpecializationResponse>());
        }

        public async Task<Result> UpdateAsync(string id, SpecializationRequest request, CancellationToken cancellation)
        {
            if (await _context.Specializations.FirstOrDefaultAsync(x => x.Id == id, cancellation) is not { } specializations)
                return Result.Failure(SpecializationError.NotFound);
            if (await _context.Specializations.AnyAsync(x => x.Name == request.Name && x.Id != id, cancellation))
                return Result.Failure(SpecializationError.NameAlreadyExists);
            specializations.Name = request.Name;
            await _context.SaveChangesAsync(cancellation);
            return Result.Success();
        }
    }
}
