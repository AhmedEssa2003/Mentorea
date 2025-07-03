using Mentorea.Abstractions.Enums;

namespace Mentorea.Entities
{
    public class Session
    {

        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string MentorId { get; set; } = null!;
        public string MenteeId { get; set; } = null!;
        public DateTime ScheduledTime { get; set; }
        public int DurationMinutes { get; set; } // Duration in minutes
        public int WaitingTime { get; set; } // Waiting time in minutes
        public int? Price { get; set; } // Price in cents
        public SessionStatus Status { get; set; } = SessionStatus.pending; // e.g., Scheduled, Completed, Cancelled
        public string Notes { get; set; } = null!; // Notes from the mentor
        public string? Comment { get; set; }  // Comment from the mentee
        public int? Rating { get; set; } // Rating given by the mentee
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow.AddHours(3);
        public DateTime? UpdatedAt { get; set; }
        public DateTime? MentorJoinedAt { get; set; } 
        public DateTime? MenteeJoinedAt { get; set; }

        public virtual ApplicationUser Mentor { get; set; } = null!;
        public virtual ApplicationUser Mentee { get; set; } = null!;

    }
}
