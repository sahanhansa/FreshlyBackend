using FreshlyBackendNew.Models;
using Microsoft.EntityFrameworkCore;

namespace FreshlyBackendNew.Data
{
    public class ApplicationDbContext : DbContext
    {
        // DbSet properties for all models
        public DbSet<Address> Addresses { get; set; }
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

        // The error indicates that the 'Feedback' class does not have a property or navigation property named 'Laundry'.
        // To fix this, you need to ensure that the 'Feedback' class has a property of type 'Laundry' and that it is properly configured in the model.

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
            // Add this configuration only if the 'Feedback' class has a 'LaundryId' foreign key and a 'Laundry' navigation property.
            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.Laundry)
                .WithMany(l => l.Feedbacks)
                .HasForeignKey(f => f.LaundryId);
        }
    }
    // Ensure the 'Feedback' class has the following properties to support the relationship with 'Laundry'.

    public class Feedback
    {
        public Guid FeedbackId { get; set; }
        public string? Description { get; set; }
        public int? Rating { get; set; }
        public Guid? OrderId { get; set; }
        public Order? Order { get; set; }

        // Add these properties to define the relationship with 'Laundry'.
        public Guid? LaundryId { get; set; }
        public Laundry? Laundry { get; set; }
    }
    // Ensure the 'Laundry' class has a collection of 'Feedback' to support the relationship.

            base.OnModelCreating(modelBuilder);
    public class Laundry
    {
        public Guid LaundryId { get; set; }
        public string? Name { get; set; }

        // Add this property to define the relationship with 'Feedback'.
        public ICollection<Feedback>? Feedbacks { get; set; }
    }
}