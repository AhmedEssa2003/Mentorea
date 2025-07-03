namespace Mentorea.Entities
{
    public class Like
    {
        
        public DateOnly CreatedAt { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow.AddHours(3));

        [ForeignKey(nameof(User))]
        public string UserId { get; set; } = null!;


        [ForeignKey(nameof(Post))]
        public string PostId { get; set; } = null!;


        public Post Post { get; set; } = null!;


        public ApplicationUser User { get; set; } = null!;
    }
}
