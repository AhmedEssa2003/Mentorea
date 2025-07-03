
using Mentorea.Helpers;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mentorea.Persistence.EntitiesConfigurations
{
    public class UserRoleConfiguration: IEntityTypeConfiguration<IdentityUserRole<string>>
    {

        public void Configure(EntityTypeBuilder<IdentityUserRole<string>> builder)
        {
            builder.HasData(new IdentityUserRole<string>
            {
                UserId = DefaultUsers.AdminId,
                RoleId = DefaultRole.AdminRoleId,
            });
            //var userRoles = ReadData.ReadCsvUserRoleData("D:\\Project\\Graduation project\\Data\\UserRole.csv");
            //builder.HasData(userRoles);

        }
    }
}
