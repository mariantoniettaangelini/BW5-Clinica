using BW5.Models;
using Microsoft.EntityFrameworkCore;

namespace ApplicationDbContex.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Animal> Animals { get; set; }
        public DbSet<Visit> Visits { get; set; }

        public DbSet<Recovery> Shelters { get; set; }

        public DbSet<Product> Products { get; set; }
        public DbSet<Sales> Sales { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Cabinet> Cabinets { get; set; }
        public DbSet<Drawer> Drawers { get; set; }
        public DbSet<ApplicationUser> Users { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
    }
}
