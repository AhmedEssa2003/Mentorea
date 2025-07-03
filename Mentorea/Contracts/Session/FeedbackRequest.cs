namespace Mentorea.Contracts.Session
{
    public record FeedbackRequest(
        int Rating,
        string Comment
    );
}
