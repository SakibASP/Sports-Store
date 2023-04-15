using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SportsStore.Models;
using SportsStore.Models.ViewModels;

namespace SportsStore.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
        {
        }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                IConfigurationRoot configuration = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json")
                   .Build();
                var connectionString = configuration.GetConnectionString("DefaultConnection");
                optionsBuilder.UseSqlServer(connectionString);
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ProductViewModel>().HasNoKey();
        }

        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Category> Category { get; set; }
        public virtual DbSet<ProductImages> ProductImages { get; set; }
        public virtual DbSet<DynamicMenuItem> DynamicMenuItem { get; set; }
        public virtual DbSet<MenuItem> MenuItem { get; set; }
        public virtual DbSet<MenuToRole> MenuToRole { get; set; }
        public virtual DbSet<ProductViewModel> ProductViewModel { get; set; }
        public virtual DbSet<ShippingDetails> ShippingDetails { get; set; }
        public virtual DbSet<ShipmentOrders> ShipmentOrders { get; set; }
        
    }
}