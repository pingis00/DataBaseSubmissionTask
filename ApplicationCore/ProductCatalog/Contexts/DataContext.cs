using System;
using System.Collections.Generic;
using ApplicationCore.ProductCatalog.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApplicationCore.ProductCatalog.Contexts;

public partial class DataContext : DbContext
{
    public DataContext()
    {
    }

    public DataContext(DbContextOptions<DataContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Brand> Brands { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Inventory> Inventories { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductReview> ProductReviews { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Brand>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Brands__3214EC071B1B4892");

            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Categori__3214EC07CCCF9732");

            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<Inventory>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__Inventor__B40CC6CDCFE8A085");

            entity.Property(e => e.ProductId).ValueGeneratedNever();

            entity.HasOne(d => d.Product).WithOne(p => p.Inventory)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Inventory__Produ__4222D4EF");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ArticleNumber).HasName("PK__Products__3214EC07658A777C");

            entity.Property(e => e.ArticleNumber).ValueGeneratedNever();

            entity.HasOne(d => d.Brand).WithMany(p => p.Products)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Products__BrandI__3E52440B");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Products__Catego__3F466844");
        });

        modelBuilder.Entity<ProductReview>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ProductR__3214EC0783B88A7C");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.ArticleNumberNavigation).WithMany(p => p.ProductReviews)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ProductRe__Produ__44FF419A");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
