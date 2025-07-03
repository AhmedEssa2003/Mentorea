using Mentorea.Abstractions.Consts;
using Mentorea.Abstractions.Enums;
using Mentorea.Entities;
using Mentorea.Helpers;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mentorea.Persistence.EntitiesConfigurations
{
    public class UserConfiguration: IEntityTypeConfiguration<ApplicationUser>
    {

        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            
            //Relationships
            builder.OwnsMany(x => x.RefreshTokens)
                .ToTable(nameof(RefreshToken))
                .WithOwner()
                .HasForeignKey("UserId");

            builder.OwnsMany(x=> x.OTPs)
                .ToTable(nameof(OTP))
                .WithOwner()
                .HasForeignKey("UserId");
            builder.OwnsOne(x => x.Card)
                .ToTable(nameof(Card))
                .WithOwner()
                .HasForeignKey("UserId");

            //Properties

            builder.Property(x => x.Name)
                .HasMaxLength(200);

            builder.Property(x => x.PathPhoto)
                .HasMaxLength(200);

            builder.Property(x => x.Location)
                .HasMaxLength(300);

            builder.Property(x => x.Gender)
                .HasMaxLength(20);

            builder.Property(x => x.About)
                .HasMaxLength(400);
            
            builder.Property(x => x.NumberOfExperience);

            builder.Property(x => x.PriceOfSession)
                .HasDefaultValue(0);

            builder.Property(x => x.PirthDate);

            builder.Property(x=>x.Gender)
                .HasConversion<string>();
            //Seed Data
            //var MenteeData = ReadData.ReadCsvMenteeData("D:\\Project\\Graduation project\\Data\\Mentees.csv");
            //builder.HasData(MenteeData);
            //var MentorData = ReadData.ReadCsvMentorData("D:\\Project\\Graduation project\\Data\\Mentors.csv");
            //builder.HasData(MentorData);

            builder.HasData(new ApplicationUser
            {
                Id = DefaultUsers.AdminId,
                Name = "Ahmed Essa",
                Email = DefaultUsers.AdminEmail,
                NormalizedEmail = DefaultUsers.AdminEmail.ToUpper(),
                UserName = DefaultUsers.AdminEmail,
                NormalizedUserName = DefaultUsers.AdminEmail.ToUpper(),
                ConcurrencyStamp = DefaultUsers.AdminEmail,
                SecurityStamp = DefaultUsers.AdminSecurityStamp,
                EmailConfirmed = true,
                PasswordHash = "AQAAAAIAAYagAAAAEOvlaWsgGXiEoyIpM1F9RwRwOdszc/u/SiXM7TMzoiT/pSVugVJ7VteUX/PbLN8jAQ==",
                Gender = Gender.Male,
                Location = "Fayoum"
                
            });
            


        }
    }
}
