namespace Mentorea.Entities
{
    public class MentorAvailability
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string MentorId { get; set; } = null!;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow.AddHours(3);
        public DateTime? UpdatedAt { get; set; }

        public ApplicationUser Mentor { get; set; } = null!;
    }
}
