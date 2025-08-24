using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Calculator.Models;

namespace Calculator.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<CalculationHistory> CalculationHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure CalculationHistory entity
            builder.Entity<CalculationHistory>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Expression).IsRequired().HasMaxLength(500);
                entity.Property(e => e.UserId).IsRequired();
                entity.Property(e => e.Result).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();

                entity.HasOne(e => e.User)
                    .WithMany(u => u.CalculationHistories)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.CreatedAt);
            });
        }
    }
}