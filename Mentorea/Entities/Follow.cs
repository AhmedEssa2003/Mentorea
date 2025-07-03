namespace Mentorea.Entities
{
    public class Follow
    {
       
        public DateOnly CreatedAt { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow.AddHours(3));
        [ForeignKey(nameof(Follower))]
        public string FollowerId { get; set; } = null!;
        [ForeignKey(nameof(Followed))]
        public string FollowedId { get; set; } = null!;
        public ApplicationUser Follower { get; set; } = null!;
        public ApplicationUser Followed { get; set; } = null!;
    }
}
