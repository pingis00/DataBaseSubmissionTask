using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using PresentationAppWpf.ProductCatalog.Entities;

namespace PresentationAppWpf.ProductCatalog.Context;

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
            entity.HasKey(e => e.Id).HasName("PK__Brands__3214EC07A72A27B1");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Categori__3214EC07A0D9CC27");
        });

        modelBuilder.Entity<Inventory>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__Inventor__B40CC6CD6650A383");

            entity.Property(e => e.ProductId).ValueGeneratedNever();
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.Property(e => e.ArticleNumber).ValueGeneratedOnAdd();

            entity.HasOne(d => d.Brand).WithMany()
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Products__BrandI__3D5E1FD2");

            entity.HasOne(d => d.Category).WithMany()
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Products__Catego__3E52440B");
        });

        modelBuilder.Entity<ProductReview>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ProductR__3214EC076CAA3401");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
