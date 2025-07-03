using Mentorea.Abstractions.Consts;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mentorea.Persistence.EntitiesConfigurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<ApplicationRole>
    {
        public void Configure(EntityTypeBuilder<ApplicationRole> builder)
        {
            builder.HasData([
                new ApplicationRole {
                    Id = DefaultRole.AdminRoleId,
                    Name = DefaultRole.Admin,
                    NormalizedName = DefaultRole.Admin.ToUpper(),
                    ConcurrencyStamp = DefaultRole.AdminRoleConcurrencyStamp,
                    IsDefault = false,
                },
                new ApplicationRole {
                    Id = DefaultRole.MenteeRoleId,
                    Name = DefaultRole.Mentee,
                    NormalizedName = DefaultRole.Mentee.ToUpper(),
                    ConcurrencyStamp = DefaultRole.MenteeRoleConcurrencyStamp,
                    IsDefault = true,
                },
                new ApplicationRole {
                    Id = DefaultRole.MentorRoleId,
                    Name = DefaultRole.Mentor,
                    NormalizedName = DefaultRole.Mentor.ToUpper(),
                    ConcurrencyStamp = DefaultRole.MentorRoleConcurrencyStamp,
                    IsDefault = true,
                }
            ]);
        }
    }
}
