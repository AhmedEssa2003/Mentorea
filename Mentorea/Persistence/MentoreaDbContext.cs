

using Mentorea.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;


namespace Mentorea.Persistence
{
    public class MentoreaDbContext(DbContextOptions<MentoreaDbContext> options) : IdentityDbContext<ApplicationUser,ApplicationRole,string>(options)
    {
        protected override void OnModelCreating(ModelBuilder builder)
        {
            var cascadeFKs = builder.Model
                .GetEntityTypes()
                .SelectMany(t => t.GetForeignKeys())
                .Where(fk => fk.DeleteBehavior == DeleteBehavior.Cascade && !fk.IsOwnership);
            foreach (var fk in cascadeFKs)
            {
                fk.DeleteBehavior = DeleteBehavior.Restrict;
            }
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(builder);
        }

        public DbSet<Field> Fields { get; set; } 
        public DbSet<MenteeFieldInterests> MenteeFields { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Follow> Follows { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<MentorAvailability> MentorAvailability { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Specialization> Specializations { get; set; }
        public DbSet<UserDevices> UserDevices { get; set; }
        public DbSet<PaymentSession> PymentSessions { get; set; }


    }
}
