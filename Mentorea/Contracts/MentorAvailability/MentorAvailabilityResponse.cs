namespace Mentorea.Contracts.MentorAvailability
{
    public record MentorAvailabilityResponse(
        string Id,
        string MentorId,
        string Date,
        string StartTime,
        string EndTime
        
    );
    
}
