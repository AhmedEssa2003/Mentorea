using Mentorea.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mentorea.Persistence.EntitiesConfigurations
{
    public class SpecializationConfiguration:IEntityTypeConfiguration<Specialization>
    {

        public void Configure(EntityTypeBuilder<Specialization> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
            //Relationships
            builder.HasMany(x=>x.Fields)
                .WithOne(x=>x.Specialization)
                .HasForeignKey(x => x.SpecializationId)
                .OnDelete(DeleteBehavior.Restrict);
            //seding Data
            //var date = ReadData.ReadCsvSpecializationData("D:\\Project\\Graduation project\\Data\\Specializations.csv");
            //builder.HasData(date);


        }
    }   
    
}

