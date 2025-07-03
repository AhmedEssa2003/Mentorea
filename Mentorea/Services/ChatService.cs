using Mentorea.Contracts.Chat;
using Mentorea.Contracts.Common;
using Mentorea.Errors;
using System.Linq.Dynamic.Core;

namespace Mentorea.Services
{
    public class ChatService(
        MentoreaDbContext context,
        IHttpContextAccessor httpContextAccessor,
        IFileService fileService
        ) : IChatService
    {
        private readonly MentoreaDbContext _context = context;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly IFileService _fileService = fileService;

        public async Task<Result<string>> SaveFileAsync(FileRequest request)
        {
            
                var fileName = await _fileService.SaveFileAsync(request.File);
                var filePath = $"{_fileService.GetBaseUrl(_httpContextAccessor)}/Chat/{fileName}";
                return Result.Success(filePath);
            
            
        }
        public async Task<Result<MessageResponse>> SaveMessage(string senderId, MessageRequest request)
        {

            if (!await _context.Users.AnyAsync(x => x.Id == request.receiverId))
                return Result.Failure<MessageResponse>(ChatError.UserNotFound);
            Message message = new Message();
            if (request.type == "text")
            {
                message.SenderId = senderId;
                message.ReceiverId = request.receiverId;
                message.Type = request.type;
                message.Content = request.content;
            }
            else
            {
                if (!_fileService.IsExist(Path.GetFileName(request.content)))
                    return Result.Failure<MessageResponse>(ChatError.FileNotFound);
                message.SenderId = senderId;
                message.ReceiverId = request.receiverId;
                message.Type = request.type;
                message.Content = request.content;
            }
            await _context.Messages.AddAsync(message);
            await _context.SaveChangesAsync();
            var response = new MessageResponse
            (
                message.Id,
                message.SenderId,
                request.receiverId,
                message.Content,
                message.Type,
                message.CreatedAt
            );
            return Result.Success(response);
        }
        public async Task<Result> DeleteMessage(string messageId, string userId)
        {
            var message = await _context.Messages.SingleOrDefaultAsync(x=>x.Id==messageId);
            if (message == null || (message.IsDeletedByReceiver && message.IsDeletedBySender))
                return Result.Failure(ChatError.MessageNotFound);
            if (message.SenderId != userId && message.ReceiverId != userId)
                return Result.Failure(ChatError.NotAuthorized);
            if (message.SenderId == userId)
                message.IsDeletedBySender = true;
            else if (message.ReceiverId == userId)
                message.IsDeletedByReceiver = true;
            await _context.SaveChangesAsync();
            return Result.Success();
        }
        public async Task<Result<PaginatedList<MessageResponse>>> GetChatHistoryAsync(RequestFilters requestFilters,string FUserId, string SUserId)
        {
            if (!await _context.Users.AnyAsync(x => x.Id == SUserId))
                return Result.Failure<PaginatedList<MessageResponse>>(ChatError.UserNotFound);
            var query = _context.Messages
                .Where(m => ((m.SenderId == FUserId && m.ReceiverId == SUserId && !m.IsDeletedBySender) ||
                            (m.SenderId == SUserId && m.ReceiverId == FUserId && !m.IsDeletedByReceiver))
                            && (string.IsNullOrEmpty(requestFilters.SearchValue) || m.Content.Contains(requestFilters.SearchValue)))
                .OrderBy($"CreatedAt {requestFilters.SortDirection}")
                .ProjectToType<MessageResponse>();

            var response = await PaginatedList<MessageResponse>.CreateAsync(
                query.AsNoTracking(),requestFilters.PageNumber,requestFilters.PageSize);
            return Result.Success(response);
        } 
        public async Task<Result>DeleteChatAsync(string FUserId, string SUserId)
        {
            if (!await _context.Users.AnyAsync(x => x.Id == SUserId))
                return Result.Failure<PaginatedList<MessageResponse>>(ChatError.UserNotFound);

            var messages = await _context.Messages
                .Where(m => (m.SenderId == FUserId && m.ReceiverId == SUserId && !m.IsDeletedBySender) || (m.SenderId == SUserId && m.ReceiverId == FUserId && !m.IsDeletedByReceiver))
                .ToListAsync();
            
            foreach (var message in messages)
            {
                if (message.SenderId == FUserId)
                    message.IsDeletedBySender = true;
                if (message.ReceiverId == FUserId)
                    message.IsDeletedByReceiver = true;
            }
            await _context.SaveChangesAsync();
            return Result.Success();
        }
    }
}
