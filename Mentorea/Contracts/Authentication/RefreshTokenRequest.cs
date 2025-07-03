namespace Mentorea.Contracts.Authentication
{
    public record RefreshTokenRequest(
        string RefreshToken,
        string Token
    );
}
