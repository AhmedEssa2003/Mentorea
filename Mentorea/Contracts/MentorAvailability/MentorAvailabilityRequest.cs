using System.Globalization;

namespace Mentorea.Contracts.MentorAvailability
{
    using System.Globalization;
    using System.Text.RegularExpressions;

    public class MentorAvailabilityRequest
    {
        public string Date { get; }
        public string StartTime { get; }
        public string EndTime { get; }

        private DateOnly? ParsedDate { get; set; }
        private TimeOnly? ParsedStartTime { get; set; }
        private TimeOnly? ParsedEndTime { get; set; }

        public DateTime? StartDateTime { get; private set; }
        public DateTime? EndDateTime { get; private set; }

        public MentorAvailabilityRequest(string date, string startTime, string endTime)
        {
            Date = date;
            StartTime = startTime;
            EndTime = endTime;

            TryParse();
        }

        private void TryParse()
        {
            if (Regex.IsMatch(StartTime,RegexPatterns.Time))
                ParsedStartTime = TimeOnly.ParseExact(StartTime, "HH:mm", CultureInfo.InvariantCulture);
            if (Regex.IsMatch(EndTime, RegexPatterns.Time))
                ParsedEndTime = TimeOnly.ParseExact(EndTime, "HH:mm", CultureInfo.InvariantCulture);
            if (Regex.IsMatch(Date, RegexPatterns.Date))
                ParsedDate = DateOnly.ParseExact(Date, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            // If all parsed successfully, build StartDateTime and EndDateTime
            if (ParsedDate.HasValue && ParsedStartTime.HasValue && ParsedEndTime.HasValue)
            {
                StartDateTime = new DateTime(ParsedDate.Value.Year, ParsedDate.Value.Month, ParsedDate.Value.Day, ParsedStartTime.Value.Hour, ParsedStartTime.Value.Minute, 0);
                EndDateTime = new DateTime(ParsedDate.Value.Year, ParsedDate.Value.Month, ParsedDate.Value.Day, ParsedEndTime.Value.Hour, ParsedEndTime.Value.Minute, 0);
            }
            else
            {
                StartDateTime = null;
                EndDateTime = null;
            }
        }
    }

}
