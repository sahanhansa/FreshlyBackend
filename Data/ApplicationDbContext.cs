using FreshlyBackendNew.Models;
using Microsoft.EntityFrameworkCore;

namespace FreshlyBackendNew.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        // DbSet properties for all models
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<DeletedCustomer> DeletedCustomers { get; set; }
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
                
            // Configure Feedback-Laundry relationship
            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.Laundry)
                .WithMany(l => l.Feedbacks)
                .HasForeignKey(f => f.LaundryId);
                
            // Configure Feedback-Customer relationship
            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.Customer)
                .WithMany(c => c.Feedbacks)
                .HasForeignKey(f => f.CustomerId);
                
            // Configure Contact-Customer relationship
            modelBuilder.Entity<Contact>()
                .HasQueryFilter(c => c.UserType == "Customer")
                .HasOne<Customer>()
                .WithMany(c => c.Contacts)
                .HasForeignKey(c => c.UserId)
                .HasPrincipalKey(c => c.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);
                
            // Configure Order-Customer relationship
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Customer)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.CustomerId);
                
            // Configure Feedback-Order relationship
            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.Order)
                .WithMany()
                .HasForeignKey(f => f.OrderId);
        }
    }
}