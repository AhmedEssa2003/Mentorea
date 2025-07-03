using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mentorea.Persistence.EntitiesConfigurations
{
    public class UserDevicesConfiguration : IEntityTypeConfiguration<UserDevices>
    {
        public void Configure(EntityTypeBuilder<UserDevices> builder)
        {
            builder.HasKey(ud => new { ud.UserId, ud.DeviceToken });
            builder.HasOne(ud => ud.User)
                .WithMany(u => u.UserDevices)
                .HasForeignKey(ud => ud.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

}
