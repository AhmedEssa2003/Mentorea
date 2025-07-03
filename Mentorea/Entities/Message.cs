namespace Mentorea.Entities
{
    public class Message
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string SenderId { get; set; } = null!;
        public string ReceiverId { get; set; } = null!;
        public string Type { get; set; } = null!;
        public string Content { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow.AddHours(3);
        public bool IsDeletedBySender { get; set; } = false;
        public bool IsDeletedByReceiver { get; set;} = false;
    }
}
