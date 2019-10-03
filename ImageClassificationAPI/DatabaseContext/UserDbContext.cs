using Microsoft.EntityFrameworkCore;
using ImageClassificationAPI.Models;

namespace ImageClassificationAPI.Entities
{
    public class UserDbContext  : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
        {
           // Database.Migrate();
        }

        public virtual DbSet<User> Users{ get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(user => 
            {
                user.HasIndex(u => new { u.Name, u.Password, u.DeviceToken })
               .IsUnique();
            });

        }
    }
}
