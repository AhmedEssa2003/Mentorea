using Mentorea.Abstractions.Enums;

namespace Mentorea.Contracts.Session
{
    public class SessionResponse
    {
        public SessionResponse(string id, string mentorId,string mentorName, string menteeId,string menteeName, DateTime scheduledTime, int durationMinutes, string status, string? notes, decimal? price)
        {
            Id = id;
            MentorId = mentorId;
            MentorName = mentorName;
            MenteeId = menteeId;
            MenteeName = menteeName;
            StartDate = scheduledTime.ToString("dd/MM/yyyy");
            StartTime = scheduledTime.ToString("HH:mm");
            DurationMinutes = durationMinutes;
            Status = status;
            Notes = notes;
            Price = price;
        }

        public string Id { get; set; } = null!;
        public string MentorId { get; set; } = null!;
        public string MentorName { get; set; } = null!;
        public string MenteeId { get; set; } = null!;
        public string MenteeName { get; set; } = null!;
        public string StartDate { get; set; } = null!;
        public string StartTime { get; set; } = null!;
        public int DurationMinutes { get; set; }
        public string Status { get; set; } = null!;
        public string? Notes { get; set; }
        public decimal? Price { get; set; }
    }
}
