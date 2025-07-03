namespace Mentorea.Abstractions.Enums
{
    public enum SessionStatus
    {
        pending,
        rejected,
        awaiting_payment,
        accepted,
        awaiting_mentee_confirmation,
        cancelled,
        completed,
        expired,
        awaiting_feedback,
        MentorAttendedOnly,
        MenteeAttendedOnly,
        Disputed,
        ongoing
    }
}
