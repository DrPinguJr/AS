using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using dit230680c_AS.Models;

namespace dit230680c_AS.Data
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }  // ✅ Use default Identity Users
        public DbSet<AuditLog> AuditLogs { get; set; }  
        public DbSet<PasswordHistory> PasswordHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ✅ Ensure Email is Unique
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // ✅ Configure PasswordHistory relationship with IdentityUser
            modelBuilder.Entity<PasswordHistory>()
                .HasOne<IdentityUser>()
                .WithMany()
                .HasForeignKey(ph => ph.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent accidental deletions
        }
    }
}
