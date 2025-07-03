using Mentorea.Entities;

namespace Mentorea.Persistence.EntitiesConfigurations
{
    public class PymentSessionConfiguration : IEntityTypeConfiguration<PaymentSession>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<PaymentSession> builder)
        {
            builder.ToTable("PymentSessions")
                .HasKey(x => x.SessionId);
        }
    }
}
