using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Fresh_Farm_Market.Model;
using Fresh_Farm_Market.Models;

namespace Fresh_Farm_Market.Model
{
    public class UserDbContext: IdentityDbContext<User>
    {
        private readonly IConfiguration _config;

        public  UserDbContext(IConfiguration config)
        {
            _config = config;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionstring = _config.GetConnectionString("UserConnectionString");
            optionsBuilder.UseSqlServer(connectionstring);
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>().Property(m => m.OTP).IsRequired(false);
            //builder.Entity<User>().Property(m => m.OTPdateTime).IsRequired(false);
            base.OnModelCreating(builder);
        }

        public DbSet<PasswordHistory> passwordHistories { get; set; }
        public DbSet<AuditLog> auditLog { get; set; }
    }
}
