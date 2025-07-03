using Mentorea.Entities;
using Mentorea.Helpers;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mentorea.Persistence.EntitiesConfigurations
{
    public class MenteeFieldInterestsConfiguration : IEntityTypeConfiguration<MenteeFieldInterests>
    {

        public void Configure(EntityTypeBuilder<MenteeFieldInterests> builder)
        {
            builder
                .HasKey(x => new { x.FieldId, x.MenteeId });
            builder.HasOne(x => x.User)
                .WithMany(m => m.MenteeFieldInterests)
                .HasForeignKey(x => x.MenteeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Field)
                .WithMany(x => x.MenteeFieldInterests)
                .HasForeignKey(x => x.FieldId)
                .OnDelete(DeleteBehavior.Restrict);
            //var menteeFieldInterests = ReadData.ReadCsvMenteeFieldInterestsData("D:\\Project\\Graduation project\\Data\\MenteeFields.csv");
            //builder.HasData(menteeFieldInterests);


        }
    }
}
