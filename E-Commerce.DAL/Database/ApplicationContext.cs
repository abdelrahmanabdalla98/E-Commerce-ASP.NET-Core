using E_Commerce.DAL.Entity;
using E_Commerce.DAL.Entity_Extension;
using E_Commerce.DAL.ExtendedEntity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.DAL.DB_Context
{
    public class ApplicationContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


            // Customize the ASP.NET Identity model and override the defaults if needed
            builder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(e => e.FirstName).HasMaxLength(100);
                entity.Property(e => e.LastName).HasMaxLength(100);
            });
            builder.Entity<Country>(entity =>
            {
                entity.ToTable("CountryMaster");
                entity.HasKey(e => e.ID);
                entity.Property(e => e.ID).HasColumnName("ID");
                entity.Property(e => e.Name).HasColumnName("Name").HasMaxLength(50);
                entity.Property(e => e.CountryCode).HasColumnName("CountryCode").HasMaxLength(5);
            });
            builder.Entity<City>(entity =>
            {
                entity.ToTable("StateMaster");
                entity.HasKey(e => e.ID);
                entity.Property(e => e.ID).HasColumnName("ID");
                entity.Property(e => e.Name).HasColumnName("Name").HasMaxLength(50);
                entity.Property(e => e.CountryId).HasColumnName("CountryID");
            });
            builder.Entity<Area>(entity =>
            {
                entity.ToTable("CityMaster");
                entity.HasKey(e => e.ID);
                entity.Property(e => e.ID).HasColumnName("ID");
                entity.Property(e => e.Name).HasColumnName("Name").HasMaxLength(50);
                entity.Property(e => e.CityId).HasColumnName("StateID");
            });

        }
        public DbSet<SizeList> Sizes { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<PaymentCard> PaymentCards { get; set; }
        public DbSet<PayType> PayTypes { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ReturnStatus> ReturnStatuses { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<WishListItem> WishListItems { get; set; }
        public DbSet<WishList> WishLists { get; set; }

    }
}
