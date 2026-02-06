using hrms.Model;
using Microsoft.EntityFrameworkCore;

namespace hrms.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {}
        public DbSet<User> Users { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(u => u.full_name)
                      .IsRequired()
                      .HasMaxLength(150);

                entity.Property(u => u.user_role)
                    .HasConversion<String>()
                    .HasMaxLength(20);

                entity.Property(u => u.email)
                      .IsRequired()
                      .HasMaxLength(150);

                entity.Property(u => u.hash_password)
                      .IsRequired()
                      .HasMaxLength(500);

                entity.Property(u => u.image_url)
                      .IsRequired()
                      .HasMaxLength(500);

                entity.HasOne(u => u.manager)
                      .WithMany(m => m.Employees)
                      .HasForeignKey(u => u.manager_id)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
