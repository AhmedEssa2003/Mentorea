
namespace Mentorea.Authentication
{
    public interface IJwtProvider
    {
        (string Token, int ExpiredIn) GenrateToken(ApplicationUser user, string Role);
        string? ValidateToken(string Token);
    }
}
