using FreshlyBackendNew.Models;
using Microsoft.EntityFrameworkCore;

namespace FreshlyBackendNew.Data
{
    public class ApplicationDbContext : DbContext
    {
        // DbSet properties for all models
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Admin> Admins { get; set; } // Added Admin DbSet
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<DriverNote> DriverNotes { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<ItemCategory> ItemCategories { get; set; }
        public DbSet<Laundry> Laundries { get; set; }
        public DbSet<LaundryItemService> LaundryItemServices { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Owner> Owners { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<TemporaryOrder> TemporaryOrders { get; set; }
        public DbSet<TemporaryOrderDetail> TemporaryOrderDetails { get; set; }


        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Composite primary keys

            // Define composite primary key for LaundryItemService
            modelBuilder.Entity<LaundryItemService>()
                .HasKey(lis => new { lis.LaundryId, lis.ItemId, lis.ServiceId });

            // Define composite primary key for OrderDetail
            modelBuilder.Entity<OrderDetail>()
                .HasKey(od => new { od.OrderId, od.ItemId, od.ServiceId });

            // Define composite primary key for TemporaryOrderDetail
            modelBuilder.Entity<TemporaryOrderDetail>()
                .HasKey(tod => new { tod.TemporaryOrderId, tod.ItemId, tod.ServiceId });
            
            // Define the relationship between Feedback and Laundry
            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.Laundry)
                .WithMany(l => l.Feedbacks)
                .HasForeignKey(f => f.LaundryId);
                
            // Seed default admin user with BCrypt hashed password
            modelBuilder.Entity<Admin>().HasData(
                new Admin
                {
                    AdminId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    Username = "admin",
                    // Plain text password for testing
                    Password = "admin123",
                    FirstName = "System",
                    LastName = "Administrator",
                    Email = "admin@freshly.com",
                    Role = "SuperAdmin",
                    CreatedAt = DateTime.UtcNow
                }
            );
        }
    }
}