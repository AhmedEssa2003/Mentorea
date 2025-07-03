namespace Mentorea.Contracts.Chat
{
    public record FileRequest
    {
        public IFormFile File { get; set; } = null!;
    }
}
