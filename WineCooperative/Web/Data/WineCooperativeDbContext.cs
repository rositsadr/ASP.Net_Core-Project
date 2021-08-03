using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Web.Data.Models;
using Web.Models;
using Web.Services.Products.Models;
using Web.Services.Services.Models;

namespace Web.Data
{
    public class WineCooperativeDbContext : IdentityDbContext<User>
    {
        public WineCooperativeDbContext(DbContextOptions<WineCooperativeDbContext> options)
            : base(options)
        {
        }

        public DbSet<UserAdditionalInformation> UserAdditionalInformation { get; set; }

        public DbSet<Address> Addresses { get; set; }

        public DbSet<Town> Towns { get; set; }

        public DbSet<Country> Countries { get; set; }

        public DbSet<Manufacturer> Manufacturers { get; set; }

        public DbSet<GrapeVariety> GrapeVarieties { get; set; }

        public DbSet<WineArea> WineAreas { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<Service> Services { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<Theme> Themes { get; set; }

        public DbSet<News> News { get; set; }

        public DbSet<OrderProduct> OrdersProducts { get; set; }

        public DbSet<ProductColor> ProductColors { get; set; }

        public DbSet<ProductTaste> ProductTastes { get; set; }

        public DbSet<ProductGrapeVariety> ProductGrapeVarieties { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<UserAdditionalInformation>()
                .HasOne(u => u.User)
                .WithOne(u => u.UserData)
                .HasForeignKey<UserAdditionalInformation>(u => u.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Manufacturer>()
                .HasOne(m=>m.User)
                .WithMany(u=>u.Manufacturers)
                .HasForeignKey(m => m.UserId)
                .OnDelete(DeleteBehavior.Restrict);           

            builder.Entity<OrderProduct>()
                .HasKey(op => new { op.OrderId, op.ProductId });

            builder.Entity<Order>()
                .HasMany(o => o.OrderProducts)
                .WithOne(op => op.Order)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Product>()
                .HasMany(p => p.ProductOrders)
                .WithOne(po => po.Product)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Product>()
                .Property(p => p.Price)
                .HasPrecision(10, 2);

            builder.Entity<ProductGrapeVariety>()
                .HasKey(x => new { x.ProductId, x.GrapeVarietyId });

            builder.Entity<ProductGrapeVariety>()
                .HasOne(pg => pg.Product)
                .WithMany(p => p.GrapeVarieties)
                .HasForeignKey(pg => pg.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ProductGrapeVariety>()
                .HasOne(pg => pg.GrapeVariety)
                .WithMany(gv => gv.Products)
                .HasForeignKey(pg => pg.GrapeVarietyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Service>()
                .Property(p => p.Price)
                .HasPrecision(10, 2);

            builder.Entity<Manufacturer>()
                .HasIndex(u => u.Name)
                .IsUnique();

            builder.Entity<Country>()
                .HasIndex(c => c.CountryName)
                .IsUnique();

            builder.Entity<GrapeVariety>()
                .HasIndex(gv => gv.Name)
                .IsUnique();

            builder.Entity<WineArea>()
                .HasIndex(wa => wa.Name)
                .IsUnique();

            base.OnModelCreating(builder);
        }
    }
}
