using Mentorea.Abstractions.Enums;

namespace Mentorea.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; } = null!;
        public DateOnly PirthDate { get; set; }
        public string? PathPhoto { get; set; }
        public string Location { get; set; } = null!;
        public Gender Gender { get; set; }
        public decimal? Rate { get; set; }
        public int? PriceOfSession { get; set; }
        public int? NumberOfSession { get; set; }
        public int? NumberOfExperience { get; set; }
        public string? About { get; set; }

        public bool IsDisabled {  get; set; }

        public string? FieldId { get; set; } 
        public Field Field { get; set; } = null!;

        public List<MenteeFieldInterests> MenteeFieldInterests { get; set; } = [];

        public List<RefreshToken> RefreshTokens { get; set; } = [];
        public List<OTP> OTPs { get; set; } = [];
        public List<Post> Posts { get; set; } = [];
        public List<Comment> Comments { get; set; } = [];
        public List<Like> Likes { get; set; } = [];
        public List<Follow> Followeds { get; set; } = [];
        public List<Follow> Followers { get; set; } = [];
        public List<Session> MentorSessions { get; set; } = [];
        public List<Session> MenteeSessions { get; set; } = [];
        public List<MentorAvailability> MentorAvailability { get; set; } = [];
        public List<UserDevices> UserDevices { get; set; } = [];
        public Card? Card { get; set; } 
    }
}
