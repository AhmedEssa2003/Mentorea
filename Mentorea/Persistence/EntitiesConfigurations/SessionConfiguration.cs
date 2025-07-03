using Mentorea.Helpers;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mentorea.Persistence.EntitiesConfigurations
{
    public class SessionConfiguration: IEntityTypeConfiguration<Session>
    {

        public void Configure(EntityTypeBuilder<Session> builder)
        {
            builder.HasKey(s => s.Id);
            builder.Property(s => s.ScheduledTime).IsRequired();
            builder.Property(s => s.DurationMinutes).IsRequired();
            builder.Property(s => s.Status).IsRequired();
            builder.Property(s => s.CreatedAt).IsRequired();
            builder.Property(s => s.MentorId).IsRequired();
            builder.Property(s => s.MenteeId).IsRequired();
            builder.Property(s=>s.Status)
                .HasConversion<string>();
            
            builder.Property(x=>x.WaitingTime)
                .HasDefaultValue(10);
            //Relationships
            builder.HasOne(s => s.Mentor)
                .WithMany(u => u.MentorSessions)
                .HasForeignKey(s => s.MentorId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(s=>s.Mentee)
                .WithMany(u=>u.MenteeSessions)
                .HasForeignKey(s => s.MenteeId)
                .OnDelete(DeleteBehavior.Restrict);

            //Seeding Data
            //var sessions = ReadData.ReadCsvSessionData("D:\\Project\\Graduation project\\Data\\Sessions.csv");
            //builder.HasData(sessions);

        }
    }
    
}
