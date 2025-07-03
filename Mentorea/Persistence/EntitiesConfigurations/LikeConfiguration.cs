using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mentorea.Persistence.EntitiesConfigurations
{
    public class LikeConfiguration : IEntityTypeConfiguration<Like>
    {
        public void Configure(EntityTypeBuilder<Like> builder)
        {
            builder.HasKey(l => new { l.UserId, l.PostId });
        }
    }
}
