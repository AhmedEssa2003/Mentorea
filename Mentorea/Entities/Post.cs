
namespace Mentorea.Entities
{
    public class Post
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Content { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow.AddHours(3);
        public DateTime? UpdatedAt { get; set; } = null;
        [ForeignKey(nameof(User))]
        public string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;

        public List<Comment> Comments { get; set; } = [];
        public List<Like> Likes { get; set; } = [];
    }
}
