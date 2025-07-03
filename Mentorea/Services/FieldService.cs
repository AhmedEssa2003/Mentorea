using Mentorea.Contracts.Fields;
using Mentorea.Entities;
using Mentorea.Errors;
using System.Collections.Generic;

namespace Mentorea.Services
{
    public class FieldService(MentoreaDbContext context):IFieldService
    {
        private readonly MentoreaDbContext _context = context;

        public async Task<Result<List<FieldResponse>>>GetAll(CancellationToken cancellationToken)
        {
            var response =await (from S in _context.Specializations
                                 join F in _context.Fields
                                 on S.Id equals F.SpecializationId
                                 select new
                                 {
                                     F.Id,
                                     F.FieldName,
                                     S.Name
                                 })
                                 .GroupBy(x => x.Name)
                                 .Select(x => new FieldResponse
                                 (
                                     x.Key,
                                     x.Select(y => new FieldSheep
                                     (
                                        y.Id,
                                        y.FieldName
                                     )).ToList()
                                 )).ToListAsync(cancellationToken);

                return Result.Success(response);
        }
        public async Task<Result<SingleFieldResponse>> GetAsync(string Id ,CancellationToken cancellationToken)
        {
            var response = await (from S in _context.Specializations
                                  join F in _context.Fields
                                  on S.Id equals F.SpecializationId
                                  where F.Id == Id
                                  select new
                                  {
                                      F.Id,
                                      F.FieldName,
                                      S.Name
                                  }).Select(x => new SingleFieldResponse
                                  (
                                      x.Id,
                                      x.FieldName,
                                      x.Name
                                  )).SingleOrDefaultAsync(cancellationToken);
            return response is null
                ? Result.Failure<SingleFieldResponse>(FieldError.NotFound)
                : Result.Success(response);

        }
        public async Task<Result<SingleFieldResponse>> CreateAsync(FieldRequest request, CancellationToken cancellationToken)
        {

            if (await _context.Specializations
                .SingleOrDefaultAsync(x => x.Id == request.SpcializationId, cancellationToken) is not { } specialization)
                return Result.Failure<SingleFieldResponse>(SpecializationError.NotFound);
            if (await _context.Fields.
                AnyAsync(x => x.FieldName == request.FieldName && x.SpecializationId == request.SpcializationId, cancellationToken))
                return Result.Failure<SingleFieldResponse>(FieldError.Duplicate);
            var field = new Field
            {
                FieldName = request.FieldName,
                SpecializationId = request.SpcializationId
            };  
            await _context.Fields.AddAsync(field,cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            var response = new SingleFieldResponse
            (
                field.Id,
                field.FieldName,
                specialization.Name
            );
            return Result.Success(response.Adapt<SingleFieldResponse>());
        }
        public async Task<Result> UpdateAsync(string Id , FieldRequest request, CancellationToken cancellationToken)
        {
            if (await _context.Fields
                .SingleOrDefaultAsync(x => x.Id == Id, cancellationToken) is not { } Field)
                return Result.Failure(FieldError.NotFound);

            if (await _context.Fields.
                AnyAsync(x => x.FieldName == request.FieldName && x.SpecializationId == request.SpcializationId && x.Id != Id,
                cancellationToken))
                return Result.Failure<SingleFieldResponse>(FieldError.Duplicate);

            Field.FieldName = request.FieldName;
            Field.SpecializationId = request.SpcializationId;
            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
        public async Task<Result> DeleteAsync(string Id, CancellationToken cancellationToken)
        {
            if (await _context.Fields
                .SingleOrDefaultAsync(x => x.Id == Id, cancellationToken) is not { } Field)
                return Result.Failure(FieldError.NotFound);
            try
            {
                _context.Fields.Remove(Field);
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch 
            {
                return Result.Failure(FieldError.HasDependentField);

            }
            
            return Result.Success();
        }
    }
}
