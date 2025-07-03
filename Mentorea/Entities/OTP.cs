using Mentorea.Abstractions.Enums;

namespace Mentorea.Entities
{
    [Owned]
    public class OTP
    {
        public int Code { get; set; } 
        public OtpPurpose Purpose { get; set; } 
        public DateTime CreatedAt { get; set; }= DateTime.UtcNow.AddHours(3);
        public DateTime ExpiredAt { get; set; }
        public DateTime? RevokedAt { get; set; }

        public bool IsExpired() => RevokedAt.HasValue || DateTime.UtcNow.AddHours(3) > ExpiredAt;
    }

}
