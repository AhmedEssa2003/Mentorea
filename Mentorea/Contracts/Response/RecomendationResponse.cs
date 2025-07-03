namespace Mentorea.Contracts.Response
{
    public class RecomendationResponse
    {
        public List<MentorRecommendation>? recommendations { get; set; }
    }
    public class MentorRecommendation
    {
        public string mentor_id { get; set; } = null!;
    }
}
