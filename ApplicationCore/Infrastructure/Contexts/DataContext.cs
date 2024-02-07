using ApplicationCore.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace ApplicationCore.Infrastructure.Contexts
{
    public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
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
        }
    }
}
