using System.Globalization;
using System.Text.RegularExpressions;

namespace Mentorea.Contracts.Session
{
    public class SessionRequest
    {
        public string MentorId { get; set; } = null!;
        public string MenteeId { get; set; } = null!;
        public string StartDate { get; set; } = null!;
        public string StartTime { get; set; } = null!;
        public int DurationMinutes { get; set; }
        public int? WaitingTime { get; set; } = null!;
        public string Notes { get; set; } = null!;

        private DateOnly? ParsedDate { get; set; }
        private TimeOnly? ParsedStartTime { get; set; }

        public DateTime? ScheduledTime { get; private set; }

        public SessionRequest(string MentorId, string MenteeId, string StartDate, string StartTime, int DurationMinutes, int? WaitingTime, string Notes)
        {
            this.MentorId = MentorId;
            this.MenteeId = MenteeId;
            this.StartDate = StartDate;
            this.StartTime = StartTime;
            this.DurationMinutes = DurationMinutes;
            this.WaitingTime = WaitingTime;
            this.Notes = Notes;

            TryParse();

        }
        private void TryParse()
        {
            if (Regex.IsMatch(StartTime, RegexPatterns.Time) && Regex.IsMatch(StartDate, RegexPatterns.Date))
            {
                ParsedDate = DateOnly.ParseExact(StartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                ParsedStartTime = TimeOnly.ParseExact(StartTime, "HH:mm", CultureInfo.InvariantCulture);
            }
            if(ParsedStartTime.HasValue && ParsedDate.HasValue)
            {
                ScheduledTime = new DateTime(ParsedDate.Value.Year, ParsedDate.Value.Month, ParsedDate.Value.Day, ParsedStartTime.Value.Hour, ParsedStartTime.Value.Minute, 0);
            }
        }
    }
}
