using FreshlyBackendNew.Models;
using Microsoft.EntityFrameworkCore;

namespace FreshlyBackendNew.Data
{
    public class ApplicationDbContext : DbContext
    {
        // DbSet properties for all models
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<PasswordResetRequest> PasswordResetRequests { get; set; }
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
        public DbSet<RejectedItem> RejectedItems { get; set; }
        public DbSet<GarmentType> GarmentTypes { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure property types to match database schema
            modelBuilder.Entity<Contact>()
                .Property(c => c.ContactId)
                .HasColumnType("char(36)");
            
            modelBuilder.Entity<Contact>()
                .Property(c => c.UserId)
                .HasColumnType("char(36)")
                .IsRequired();
            
            modelBuilder.Entity<Contact>()
                .Property(c => c.UserType)
                .HasColumnType("varchar(50)")
                .IsRequired();
            
            modelBuilder.Entity<Contact>()
                .Property(c => c.ContactNumber)
                .HasColumnType("varchar(20)");

            // Define composite primary key for LaundryItemService
            modelBuilder.Entity<LaundryItemService>()
                .HasKey(lis => new { lis.LaundryId, lis.ItemId, lis.ServiceId, lis.GarmentTypeId });

            // Define composite primary key for OrderDetail
            modelBuilder.Entity<OrderDetail>()
                .HasKey(od => new { od.OrderId, od.ItemId, od.ServiceId, od.GarmentTypeId });

            // Define composite primary key for TemporaryOrderDetail
            modelBuilder.Entity<TemporaryOrderDetail>()
                .HasKey(tod => new { tod.TemporaryOrderId, tod.ItemId, tod.ServiceId, tod.GarmentTypeId });
            
            // Define the relationship between Feedback and Laundry
            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.Laundry)
                .WithMany(l => l.Feedbacks)
                .HasForeignKey(f => f.LaundryId);

            // ============================================================
            // PERFORMANCE INDEXES
            // ============================================================
            
            // Composite index for order queries
            modelBuilder.Entity<Order>()
                .HasIndex(o => new { o.LaundryId, o.StatusId, o.PlacedAt })
                .HasDatabaseName("IX_Orders_LaundryId_StatusId_PlacedAt");

            // Unique index for customer username
            modelBuilder.Entity<Customer>()
                .HasIndex(c => c.Username)
                .IsUnique()
                .HasDatabaseName("IX_Customers_Username_Unique");

            // Index for customer orders lookup
            modelBuilder.Entity<Order>()
                .HasIndex(o => o.CustomerId)
                .HasDatabaseName("IX_Orders_CustomerId");

            // Index for status-based queries
            modelBuilder.Entity<Order>()
                .HasIndex(o => o.StatusId)
                .HasDatabaseName("IX_Orders_StatusId");

            // Index for order details queries
            modelBuilder.Entity<OrderDetail>()
                .HasIndex(od => od.OrderId)
                .HasDatabaseName("IX_OrderDetails_OrderId");

            // Composite index for laundry item service pricing lookups
            modelBuilder.Entity<LaundryItemService>()
                .HasIndex(lis => new { lis.LaundryId, lis.ItemId, lis.ServiceId })
                .HasDatabaseName("IX_LaundryItemServices_LaundryId_ItemId_ServiceId");

            // Composite index for contact lookups
            modelBuilder.Entity<Contact>()
                .HasIndex(c => new { c.UserId, c.UserType })
                .HasDatabaseName("IX_Contacts_UserId_UserType");

            // Additional useful indexes
            modelBuilder.Entity<Feedback>()
                .HasIndex(f => f.LaundryId)
                .HasDatabaseName("IX_Feedbacks_LaundryId");

            modelBuilder.Entity<Feedback>()
                .HasIndex(f => f.OrderId)
                .HasDatabaseName("IX_Feedbacks_OrderId");

            modelBuilder.Entity<RejectedItem>()
                .HasIndex(ri => ri.OrderId)
                .HasDatabaseName("IX_RejectedItems_OrderId");

            modelBuilder.Entity<RejectedItem>()
                .HasIndex(ri => ri.LaundryId)
                .HasDatabaseName("IX_RejectedItems_LaundryId");

            // ============================================================
            // SEED DATA
            // ============================================================
                
            // Seed default admin user with BCrypt hashed password
            modelBuilder.Entity<Admin>().HasData(
                new Admin
                {
                    AdminId = Guid.Parse("af901ac5-4e1a-42fd-a861-f0d444c83b2b"),
                    Username = "Sahan",
                    Password = "$2a$11$QJaprhObdDnV1UX87e.Ef.avrEdy29ywB337joSNdrqnY3LszunAW",
                    FirstName = "Sahan",
                    LastName = string.Empty,
                    Email = string.Empty,
                    Role = "SuperAdmin",
                    CreatedAt = DateTime.Parse("2025-07-21T19:29:35.207401Z")
                }
            );
        }
    }
}