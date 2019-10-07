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
        public virtual DbSet<Photo> Photos { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(user => 
            {
                user.HasIndex(u => new { u.Name, u.Password, u.DeviceToken })
               .IsUnique();
            });
            modelBuilder.Entity<Photo>(photo =>
            {
                photo.HasOne(p => p.User)
                    .WithMany(u => u.Photos)
                    .HasForeignKey(p => p.UserId);
            });
        }
    }
}
