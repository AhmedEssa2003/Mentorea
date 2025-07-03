using Mentorea.Contracts.Chat;
using Mentorea.Contracts.Common;

namespace Mentorea.Services
{
    public interface IChatService
    {
        Task<Result<string>> SaveFileAsync(FileRequest request);
        Task<Result<MessageResponse>> SaveMessage(string senderId, MessageRequest request);
        Task<Result> DeleteMessage(string messageId, string userId);
        Task<Result<PaginatedList<MessageResponse>>> GetChatHistoryAsync(RequestFilters requestFilters, string senderId, string receiverId);
        Task<Result> DeleteChatAsync(string FUserId, string SUserId);
    }
}
