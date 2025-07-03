using Hangfire;
using Mentorea.Contracts.Chat;
using Microsoft.AspNetCore.SignalR;

namespace Mentorea.Hubs
{
    [Authorize]
    public class ChatHub(IChatService chatService,IFcmService fcmService) : Hub
    {
        private readonly IChatService _chatService = chatService;
        private readonly IFcmService _fcmService = fcmService;
        private static readonly Dictionary<string, List<string>> _onlineUser = new();
        public override Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier!;
            lock (_onlineUser)
            {
                if (!_onlineUser.ContainsKey(userId))
                    _onlineUser[userId] = new List<string>();

                _onlineUser[userId].Add(Context.ConnectionId);
            }
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.UserIdentifier!;
            lock (_onlineUser)
            {
                if (_onlineUser.ContainsKey(userId))
                {
                    _onlineUser[userId].Remove(Context.ConnectionId);
                    if (_onlineUser[userId].Count == 0)
                        _onlineUser.Remove(userId);
                }
            }
            return base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(MessageRequest request)
        {
            var userId = Context.UserIdentifier!;
            var result = await _chatService.SaveMessage(userId, request);
            if (result.IsFailure)
            {
                await Clients.Caller.SendAsync("error", result.Error.Description);
                return;
            }

            var message = result.Value();

            try
            {
                if (_onlineUser.ContainsKey(userId))
                    await Clients.Clients(_onlineUser[userId]).SendAsync("newmessage", message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending message: {ex.Message}");
            }

            if (_onlineUser.ContainsKey(request.receiverId))
            {
                await Clients.Clients(_onlineUser[request.receiverId]).SendAsync("newmessage", message);
            }
            else
            {
                BackgroundJob.Enqueue(() => _fcmService.SendPushNotificationToUserDevicesAsync(
                    request.receiverId,
                    "New Message Received",
                    "You have received a new message. Open the app to read it",
                    CancellationToken.None
                ));
            }
        }

    }
}
