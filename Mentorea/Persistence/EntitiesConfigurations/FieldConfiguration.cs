using Mentorea.Entities;
using Mentorea.Helpers;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mentorea.Persistence.EntitiesConfigurations
{
    public class FieldConfiguration : IEntityTypeConfiguration<Field>
    {

        public void Configure(EntityTypeBuilder<Field> builder)
        {
            builder.Property(x => x.FieldName)
                .IsRequired()
                .HasMaxLength(70);

            //Relationships
            builder.HasMany(x=>x.Users)
                .WithOne(x => x.Field)
                .HasForeignKey(x => x.FieldId)
                .OnDelete(DeleteBehavior.Restrict);
            //Seeding Data

            //var date = ReadData.ReadCsvFieldData("D:\\Project\\Graduation project\\Data\\Fields.csv");
            //builder.HasData(date);


        }

    }
}
