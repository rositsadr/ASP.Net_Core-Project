using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Web.Data.Models;
using Web.Models;

namespace Web.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
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

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<UserAdditionalInformation>()
                .HasOne(u => u.User)
                .WithOne(u => u.UserData)
                .HasForeignKey<UserAdditionalInformation>(u => u.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Manufacturer>()
                .HasOne(m => m.User)
                .WithOne(u => u.Manufacturer)
                .HasForeignKey<Manufacturer>(m => m.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Product>().Property(p => p.Price).HasPrecision(10, 2);

            builder.Entity<Service>().Property(p => p.Price).HasPrecision(10, 2);

            base.OnModelCreating(builder);
        }
    }
}
