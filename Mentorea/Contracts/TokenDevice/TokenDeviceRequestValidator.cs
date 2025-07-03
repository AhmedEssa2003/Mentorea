namespace Mentorea.Contracts.TokenDevice
{
    public class TokenDeviceRequestValidator:AbstractValidator<TokenDeviceRequest>
    {
        public TokenDeviceRequestValidator()
        {
            RuleFor(x => x.DeviceToken)
                .NotEmpty()
                .WithMessage("Device token is required.");
            RuleFor(x => x.UserId)
                .NotEmpty()
                .WithMessage("User ID is required.");
        }
    }
    
}
