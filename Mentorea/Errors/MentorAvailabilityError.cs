namespace Mentorea.Errors
{
    public static class MentorAvailabilityError
    {
        public static Error NotFound => new Error("MentorAvailability.NotFound", "Mentor availability not found",StatusCodes.Status404NotFound);
        public static Error OverlappingTime => new Error("MentorAvailability.OverlappingTime", "Mentor availability time overlaps with existing availability", StatusCodes.Status409Conflict);
        public static Error MentorNotFound => new Error("MentorAvailability.MentorNotFound", "Mentor not found", StatusCodes.Status404NotFound);
        public static Error InvalidTime => new Error("MentorAvailability.InvalidTime", "Invalid start or end time", StatusCodes.Status400BadRequest);

    }
}
