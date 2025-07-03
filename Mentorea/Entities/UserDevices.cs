namespace Mentorea.Entities
{
    public class UserDevices
    {
        public string UserId { get; set; } = null!;
        public string DeviceToken { get; set; } = null!;
        public bool IsActive { get; set; } = true;
        public ApplicationUser User { get; set; } = null!;
    }
}
