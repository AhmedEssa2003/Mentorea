using Google.Apis.Auth.OAuth2;
using Mentorea.Contracts.TokenDevice;
using Mentorea.Errors;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Mentorea.Services
{
    public class FcmService(
        HttpClient httpClient,
        IConfiguration config,
        MentoreaDbContext context
        ) : IFcmService
    {
        private readonly HttpClient _httpClient = httpClient;
        private readonly IConfiguration _config = config;
        private readonly MentoreaDbContext _context = context;

        public async Task SendPushNotificationToUserDevicesAsync(string userId, string title, string body, CancellationToken cancellationToken = default)
        {
            var deviceTokens = await _context.UserDevices
                .Where(x => x.UserId == userId && x.IsActive)
                .Select(x => x.DeviceToken)
                .ToListAsync(cancellationToken);

            foreach (var token in deviceTokens)
            {
                await SendPushNotificationAsync(token, title, body, cancellationToken);
            }
        }
        public async Task<Result> AddTokenDevice(TokenDeviceRequest request, CancellationToken cancellationToken = default)
        {
            

            var existingDevice = await _context.UserDevices
                .FirstOrDefaultAsync(x => x.UserId == request.UserId
                    && x.DeviceToken == request.DeviceToken, cancellationToken);

            if (existingDevice is not null)
            {
                if (existingDevice.IsActive)
                    return Result.Success();

                existingDevice.IsActive = true;
                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }

            if(!await _context.Users.AnyAsync(x=> x.Id == request.UserId && x.EmailConfirmed && !x.IsDisabled,cancellationToken ))
                return Result.Failure(UserError.NotFoundUser);

            var newUserDevice = new UserDevices
            {
                DeviceToken = request.DeviceToken,
                UserId = request.UserId,
                IsActive = true
            };
            await _context.UserDevices.AddAsync(newUserDevice, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }


        private async Task SendPushNotificationAsync(string deviceToken, string title, string body,CancellationToken cancellationToken = default)
        {
            var accessToken = await GetAccessTokenAsync();
            var projectId = _config["Fcm:ProjectId"];
            var message = new
            {
                message = new
                {
                    token = deviceToken,
                    notification = new
                    {
                        title,
                        body
                    }
                }
            };
            var request = new HttpRequestMessage(HttpMethod.Post, $"https://fcm.googleapis.com/v1/projects/{projectId}/messages:send");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            request.Content = new StringContent(JsonSerializer.Serialize(message), System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.SendAsync(request,cancellationToken);
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                try
                {
                    var userDevice = await _context.UserDevices
                        .FirstAsync(x => x.DeviceToken == deviceToken, cancellationToken);
                    userDevice.IsActive = false;
                    await _context.SaveChangesAsync(cancellationToken);
                }
                catch (InvalidOperationException)
                {
                    throw new Exception("Device token not found in the database.");
                }

            }
        }
        private async Task<string> GetAccessTokenAsync()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(),_config.GetValue<string>("Fcm:ServiceAccountFile")!);
            var jsonContent = await File.ReadAllTextAsync(filePath);
            using var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(jsonContent!));
            var credential = GoogleCredential.FromStream(memoryStream)
                                  .CreateScoped("https://www.googleapis.com/auth/firebase.messaging");

            return await credential.UnderlyingCredential.GetAccessTokenForRequestAsync();
        }
    }
}
