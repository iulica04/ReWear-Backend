using Domain.Entities;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<ClothingItem> ClothingItems { get; set; }
        public DbSet<ClothingTag> ClothingTags { get; set; }
        public DbSet<Outfit> Outfits { get; set; }
        public DbSet<OutfitClothingItem> OutfitClothingItems { get; set; }
        public DbSet<PasswordResetCode> PasswordResetCodes { get; set; }
        public DbSet<FavoriteOutfit> FavoriteOutfits { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasPostgresExtension("uuid-ossp");

            // User
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
                entity.Property(e => e.PasswordHash);
                entity.Property(e => e.Role).IsRequired();
                entity.Property(e => e.LoginProvider).IsRequired();
                entity.Property(e => e.GoogleId).HasMaxLength(50);
                entity.Property(e => e.ProfilePicture).HasMaxLength(200);
            });

            // ClothingItem
            modelBuilder.Entity<ClothingItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnType("uuid");
                entity.Property(e => e.UserId).IsRequired().HasColumnType("uuid");
                entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Category).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Color).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Brand).IsRequired().HasMaxLength(30);
                entity.Property(e => e.Material).IsRequired().HasMaxLength(30);
                entity.Property(e => e.PrintType).HasMaxLength(30);
                entity.Property(e => e.PrintDescription).HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(2000);
                entity.Property(e => e.FrontImageUrl).IsRequired();
                entity.Property(e => e.BackImageUrl).IsRequired();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.Embedding);
                entity.Property(e => e.NumberOfWears).HasDefaultValue((uint)0);

                entity.HasMany(e => e.Tags)
                    .WithOne(t => t.ClothingItem)
                    .HasForeignKey(t => t.ClothingItemId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ClothingTag
            modelBuilder.Entity<ClothingTag>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnType("uuid").HasDefaultValueSql("uuid_generate_v4()");
                entity.Property(e => e.Tag).IsRequired().HasMaxLength(30);
            });

            // Outfit
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
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // OutfitClothingItem (Many-to-Many)
            modelBuilder.Entity<OutfitClothingItem>()
                .HasKey(oci => new { oci.OutfitId, oci.ClothingItemId });

            modelBuilder.Entity<OutfitClothingItem>()
                .HasOne(oci => oci.Outfit)
                .WithMany(o => o.OutfitClothingItems)
                .HasForeignKey(oci => oci.OutfitId);

            modelBuilder.Entity<OutfitClothingItem>()
                .HasOne(oci => oci.ClothingItem)
                .WithMany(ci => ci.OutfitClothingItems)
                .HasForeignKey(oci => oci.ClothingItemId);

            // PasswordResetCode
            modelBuilder.Entity<PasswordResetCode>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id)
                    .HasColumnType("uuid")
                    .HasDefaultValueSql("uuid_generate_v4()");
                entity.Property(e => e.UserId).IsRequired().HasColumnType("uuid");
                entity.Property(e => e.Code).IsRequired().HasMaxLength(100);
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.Attempts).IsRequired();
                entity.Property(e => e.IsUsed).IsRequired();
            });

            // FavoriteOutfit
            modelBuilder.Entity<FavoriteOutfit>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id)
                    .HasColumnType("uuid")
                    .HasDefaultValueSql("uuid_generate_v4()");
                entity.Property(e => e.UserId).IsRequired().HasColumnType("uuid");
                entity.Property(e => e.OutfitId).IsRequired().HasColumnType("uuid");

                entity.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne<Outfit>()
                    .WithMany()
                    .HasForeignKey(e => e.OutfitId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}