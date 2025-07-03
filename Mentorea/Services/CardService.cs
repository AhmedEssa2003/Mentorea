using Mentorea.Contracts.Card;
using Mentorea.Errors;

namespace Mentorea.Services
{
    public class CardService(MentoreaDbContext context) : ICardService
    {
        private readonly MentoreaDbContext _context = context;

        public async Task<Result<string>> CreateAsync(string userId ,CardRequest request,CancellationToken cancellationToken)
        {
            var user = await _context.Users.SingleOrDefaultAsync(x => x.Id == userId, cancellationToken);
            if (user!.Card.CardId is not null)
                return Result.Failure<string>(CardError.AlreadyExist);
            user.Card.CardId = request.CardId;
            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success(request.CardId);
        }
        public async Task<Result<string>> GetAsync(string userId ,CancellationToken cancellationToken)
        {
            if (await _context.Users.SingleOrDefaultAsync(x=>x.Id == userId , cancellationToken) is not { } user)
                return Result.Failure<string>(UserError.NotFoundUser);
            if (user.Card.CardId is null)
                return Result.Failure<string>(CardError.NotFoundCardId);
            return Result.Success(user.Card.CardId);

        }
        public async Task<Result> UpdateAsync(string userId ,CardRequest request ,CancellationToken cancellationToken)
        {
            var user = await _context.Users.SingleAsync(x=>x.Id==userId, cancellationToken);
            if (user.Card.CardId is null)
                return Result.Failure<string>(CardError.NotFoundCardId);
            user.Card.CardId= request.CardId;
            await _context.SaveChangesAsync(cancellationToken );
            return Result.Success();
        }
        public async Task<Result> DeleteAsync(string userId ,CancellationToken cancellationToken)
        {
            var user = await _context.Users.SingleAsync(x => x.Id == userId, cancellationToken);
            if (user.Card.CardId is null)
                return Result.Failure<string>(CardError.NotFoundCardId);
            user.Card.CardId= null;
            await _context.SaveChangesAsync(cancellationToken );
            return Result.Success();
        }
    }
}
