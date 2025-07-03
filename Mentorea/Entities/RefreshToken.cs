namespace Mentorea.Entities
{
    [Owned]
    public class RefreshToken
    {
        public string Token { get; set; } = null!;
        public DateTime ExpiresOn { get; set; }
        public DateTime CreateOn { get; set; } = DateTime.Now;
        public DateTime? RevokedOn { get; set; }
        public bool IsExpired => DateTime.UtcNow.AddHours(3) >= ExpiresOn || RevokedOn.HasValue;
        
    }
}
