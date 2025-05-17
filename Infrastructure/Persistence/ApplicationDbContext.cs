using Domain.Entities;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public class ApplicationDbContext :DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<ClothingItem> ClothingItems { get; set; }
        public DbSet<ClothingTag> ClothingTags { get; set; }
        public DbSet<Outfit> Outfits { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); 

            modelBuilder.HasPostgresExtension("uuid-ossp");

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id)
                    .HasColumnType("uuid")
                    .HasDefaultValueSql("uuid_generate_v4()");
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(30);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(30);
                entity.Property(e => e.UserName).IsRequired().HasMaxLength(30);
                entity.Property(e => e.Email).IsRequired();
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.Role).IsRequired();
                
            });

            modelBuilder.Entity<ClothingItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id)
                    .HasColumnType("uuid");
                entity.Property(e => e.UserId).IsRequired().HasColumnType("uuid");
                entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Category).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Color).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Brand).IsRequired().HasMaxLength(30);
                entity.Property(e => e.Material).IsRequired().HasMaxLength(30);
                entity.Property(e => e.PrintType).HasMaxLength(30);
                entity.Property(e => e.PrintDescription).HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(200);
                entity.Property(e => e.FrontImageUrl).IsRequired();
                entity.Property(e => e.BackImageUrl).IsRequired();
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.Embedding);

                entity.HasMany(e => e.Tags)
                    .WithOne(t => t.ClothingItem)
                    .HasForeignKey(t => t.ClothingItemId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Define relationship between ClothingItem and User
                entity.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ClothingTag>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnType("uuid").HasDefaultValueSql("uuid_generate_v4()");
                entity.Property(e => e.Tag).IsRequired().HasMaxLength(30);
            });

            modelBuilder.Entity<Outfit>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id)
                    .HasColumnType("uuid")
                    .HasDefaultValueSql("uuid_generate_v4()");

                entity.Property(e => e.UserId).IsRequired().HasColumnType("uuid");
                entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Description).HasMaxLength(200);
                entity.Property(e => e.ImageUrl).IsRequired();
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                // Define relationship between Outfit and User (One-to-Many)
                entity.HasOne<User>()
                    .WithMany() // No navigation property in User
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Define relationship between Outfit and ClothingItems (One-to-Many)
                entity.HasMany(e => e.ClothingItems)
                    .WithOne() // No navigation property in ClothingItem
                    .OnDelete(DeleteBehavior.Cascade);
            });

        }
    }
}