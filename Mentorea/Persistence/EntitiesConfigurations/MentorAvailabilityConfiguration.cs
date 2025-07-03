using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mentorea.Persistence.EntitiesConfigurations
{
    public class MentorAvailabilityConfiguration : IEntityTypeConfiguration<MentorAvailability>
    {
        public void Configure(EntityTypeBuilder<MentorAvailability> builder)
        {
            builder.HasKey(ma => ma.Id);
            builder.Property(ma => ma.StartTime)
                .IsRequired();
            builder.Property(ma => ma.EndTime)
                .IsRequired();
            builder.Property(ma => ma.CreatedAt)
                .IsRequired();

            builder.HasOne(m => m.Mentor)
                .WithMany(u => u.MentorAvailability)
                .HasForeignKey(m => m.MentorId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

}
