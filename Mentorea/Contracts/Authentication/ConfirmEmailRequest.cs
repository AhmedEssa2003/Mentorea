namespace Mentorea.Contracts.Authentication
{
    public record ConfirmEmailRequest(
        string Email,
        string Code
    );
}
