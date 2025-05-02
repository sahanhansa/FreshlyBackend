using FreshlyBackendNew.Models;
using Microsoft.EntityFrameworkCore;

namespace FreshlyBackendNew.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Laundry> Laundries { get; set; }
        public DbSet<LaundryItemService> LaundryItemServices { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }
        public DbSet<OrderType> OrderTypes { get; set; }
        public DbSet<Owner> Owners { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Privilege> Privileges { get; set; }
        public DbSet<PrivilegeUserGroup> PrivilegeUserGroups { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserGroup> UserGroups { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); 

            // Ensures a new Guid is generated when adding a record
            modelBuilder.Entity<Customer>()
                .Property(c => c.CustomerId)
                .ValueGeneratedOnAdd();

            // Configure composite primary key for PrivilegeUserGroup
            modelBuilder.Entity<PrivilegeUserGroup>()
                .HasKey(pug => new { pug.PrivilegeId, pug.UserGroupId });

            // Configure many-to-many relationship between Privilege and UserGroup
            modelBuilder.Entity<PrivilegeUserGroup>()
                .HasOne(pug => pug.Privilege)
                .WithMany(p => p.PrivilegeUserGroups)
                .HasForeignKey(pug => pug.PrivilegeId);

            modelBuilder.Entity<PrivilegeUserGroup>()
                .HasOne(pug => pug.UserGroup)
                .WithMany(ug => ug.PrivilegeUserGroups)
                .HasForeignKey(pug => pug.UserGroupId);

            // Configure one-to-many relationship between UserGroup and User
            modelBuilder.Entity<User>()
                .HasOne(u => u.UserGroup)
                .WithMany(ug => ug.Users)
                .HasForeignKey(u => u.UserGroupId);

            // Configure one-to-many relationship between Status and Order
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Status)
                .WithMany(s => s.Orders)
                .HasForeignKey(o => o.StatusId);

            // Configure one-to-many relationship between OrderType and Order
            modelBuilder.Entity<Order>()
                .HasOne(o => o.OrderType)
                .WithMany(ot => ot.Orders)
                .HasForeignKey(o => o.TypeId);

            // Configure one-to-many relationship between User and Order
            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId);

            // Configure one-to-many relationship between Laundry and Order
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Laundry)
                .WithMany(l => l.Orders)
                .HasForeignKey(o => o.LaundryId);


            // Configure one-to-many relationship between Customer and Order
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Customer)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.CustomerId);

            //Configure one-to-one relationship between Payment and Order
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Payment)
                .WithOne(p => p.Order)
                .HasForeignKey<Payment>(p => p.OrderId);
            
            // Configure one-to-many relationships between Contact and others
            modelBuilder.Entity<Contact>()
                .HasOne(c => c.Customer)
                .WithMany(cu => cu.Contacts)
                .HasForeignKey(c => c.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Contact>()
                .HasOne(c => c.Owner)
                .WithMany(o => o.Contacts)
                .HasForeignKey(c => c.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Contact>()
                .HasOne(c => c.Laundry)
                .WithMany(l => l.Contacts)
                .HasForeignKey(c => c.LaundryId)
                .OnDelete(DeleteBehavior.Cascade);


            // Configure one-to-many relationship between Laundry and Feedback
            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.Laundry)
                .WithMany(l => l.Feedbacks)
                .HasForeignKey(f => f.LaundryId);

            // Configure one-to-many relationship between Customer and Feedback
            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.Customer)
                .WithMany(c => c.Feedbacks)
                .HasForeignKey(f => f.CustomerId);

            // Configure Customer-Address relationship
            modelBuilder.Entity<Customer>()
                .HasOne(c => c.Address)
                .WithOne(a => a.Customer)
                .HasForeignKey<Customer>(c => c.AddressId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Laundry-Address relationship
            modelBuilder.Entity<Laundry>()
                .HasOne(l => l.Address)
                .WithOne(a => a.Laundry)
                .HasForeignKey<Laundry>(l => l.AddressId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure one-to-one relationship between Owner and Laundry
            modelBuilder.Entity<Laundry>()
                .HasOne(l => l.Owner)
                .WithOne(o => o.Laundry)
                .HasForeignKey<Laundry>(l => l.OwnerId);

            // Configure one-to-one relationship between Address and Owner
            modelBuilder.Entity<Owner>()
                .HasOne(o => o.Address)
                .WithOne(a => a.Owner)
                .HasForeignKey<Owner>(o => o.AddressId);

            // Configure composite primary key for OrderDetails
            modelBuilder.Entity<OrderDetails>()
                .HasKey(od => new { od.OrderId, od.ItemId, od.ServiceId });

            // Configure relationships
            modelBuilder.Entity<OrderDetails>()
                .HasOne(od => od.Order)
                .WithMany(o => o.OrderDetails)
                .HasForeignKey(od => od.OrderId);

            modelBuilder.Entity<OrderDetails>()
                .HasOne(od => od.Item)
                .WithMany(i => i.OrderDetails)
                .HasForeignKey(od => od.ItemId);

            modelBuilder.Entity<OrderDetails>()
                .HasOne(od => od.Service)
                .WithMany(s => s.OrderDetails)
                .HasForeignKey(od => od.ServiceId);

            // Configure composite primary key for LaundryItemService
            modelBuilder.Entity<LaundryItemService>()
                .HasKey(lis => new { lis.LaundryId, lis.ItemId, lis.ServiceId });

            // Configure relationships
            modelBuilder.Entity<LaundryItemService>()
                .HasOne(lis => lis.Laundry)
                .WithMany(l => l.LaundryItemServices)
                .HasForeignKey(lis => lis.LaundryId);

            modelBuilder.Entity<LaundryItemService>()
                .HasOne(lis => lis.Item)
                .WithMany(i => i.LaundryItemServices)
                .HasForeignKey(lis => lis.ItemId);

            modelBuilder.Entity<LaundryItemService>()
                .HasOne(lis => lis.Service)
                .WithMany(s => s.LaundryItemServices)
                .HasForeignKey(lis => lis.ServiceId);
        }
    }
}
