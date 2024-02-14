using ApplicationCore.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace ApplicationCore.Infrastructure.Contexts
{
    public class EagerLoadingContext(DbContextOptions<EagerLoadingContext> options) : DbContext(options)
    {
        public virtual DbSet<RoleEntity> Roles { get; set; }
        public virtual DbSet<AddressEntity> Addresses { get; set; }
        public virtual DbSet<ContactPreferenceEntity> ContactPreferences { get; set; }
        public virtual DbSet<CustomerEntity> Customers { get; set; }
        public virtual DbSet<CustomerReviewEntity> CustomersReviews { get; set;}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RoleEntity>()
                .HasIndex(x => x.RoleName)
                .IsUnique();

            modelBuilder.Entity<CustomerEntity>()
                .HasIndex(x => x.Email)
                .IsUnique();

            modelBuilder.Entity<ContactPreferenceEntity>()
                .HasIndex(x => x.PreferredContactMethod)
                .IsUnique();

            modelBuilder.Entity<CustomerReviewEntity>()
                .HasOne(cr => cr.Customer)
                .WithMany(c => c.CustomerReviewEntities)
                .HasForeignKey(cr => cr.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
