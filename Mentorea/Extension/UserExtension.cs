using System.Security.Claims;

namespace Mentorea.Extension
{
    public static class UserExtension
    {
        public static string? GetUserId(this ClaimsPrincipal User )
        {
           return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }
}
