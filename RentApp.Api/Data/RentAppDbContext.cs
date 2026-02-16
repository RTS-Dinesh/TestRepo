using Microsoft.EntityFrameworkCore;
using RentApp.Api.Models;

namespace RentApp.Api.Data;

public class RentAppDbContext : DbContext
{
    public RentAppDbContext(DbContextOptions<RentAppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Category> Categories => Set<Category>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Listing> Listings => Set<Listing>();
    public DbSet<Rental> Rentals => Set<Rental>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Name);
            entity
                .HasOne(e => e.ParentCategory)
                .WithMany(e => e.SubCategories)
                .HasForeignKey(e => e.ParentCategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
        });

        modelBuilder.Entity<Listing>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.CategoryId);
            entity.HasIndex(e => e.OwnerId);
            entity.HasIndex(e => new { e.City, e.Country });
            entity
                .HasOne(e => e.Category)
                .WithMany(e => e.Listings)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
            entity
                .HasOne(e => e.Owner)
                .WithMany(e => e.Listings)
                .HasForeignKey(e => e.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Rental>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.ListingId);
            entity.HasIndex(e => e.RenterId);
            entity
                .HasOne(e => e.Listing)
                .WithMany(e => e.Rentals)
                .HasForeignKey(e => e.ListingId)
                .OnDelete(DeleteBehavior.Restrict);
            entity
                .HasOne(e => e.Renter)
                .WithMany(e => e.RentalsAsRenter)
                .HasForeignKey(e => e.RenterId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
