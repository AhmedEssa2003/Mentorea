using Mentorea.Contracts.TokenDevice;

namespace Mentorea.Services
{
    public interface IFcmService
    {
        Task SendPushNotificationToUserDevicesAsync(string userId, string title, string body, CancellationToken cancellationToken = default);
        Task<Result> AddTokenDevice(TokenDeviceRequest request, CancellationToken cancellationToken = default);


    }
}
