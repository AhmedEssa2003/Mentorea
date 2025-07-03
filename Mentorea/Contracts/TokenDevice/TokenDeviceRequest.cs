namespace Mentorea.Contracts.TokenDevice
{
    public record TokenDeviceRequest(
        string DeviceToken,
        string UserId
    );
}
