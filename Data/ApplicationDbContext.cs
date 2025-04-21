using FreshlyBackendNew.Models;
using Microsoft.EntityFrameworkCore;

namespace FreshlyBackendNew.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Customer> Customers { get; set; } // Plural naming convention
        public DbSet<Laundry> Laundries { get; set; }
        public DbSet<Owner> Owners { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserGroup> UserGroups { get; set; }
        public DbSet<Privilege> Privileges { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<ItemService> ItemServices { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<CustomerLaundry> CustomerLaundries { get; set; }
        public DbSet<UserLaundry> UserLaundries { get; set; }
        public DbSet<UserCustomer> UserCustomers { get; set; }
        public DbSet<UserGroupPrivilege> UserGroupPrivileges { get; set; }



        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Call the base method first

            // Ensures a new Guid is generated when adding a record
            modelBuilder.Entity<Customer>()
                .Property(c => c.CustomerId)
                .ValueGeneratedOnAdd();

            // Configure one-to-one relationship between Laundry and Owner
            modelBuilder.Entity<Laundry>()
                .HasOne(l => l.Owner)
                .WithOne(o => o.Laundry)
                .HasForeignKey<Laundry>(l => l.OwnerId)
                .OnDelete(DeleteBehavior.Cascade); // Optional: Cascade delete

            // Configure one-to-many relationship between Laundry and Item
            modelBuilder.Entity<Laundry>()
                .HasMany(l => l.Items)
                .WithOne(i => i.Laundry)
                .HasForeignKey(i => i.LaundryId)
                .OnDelete(DeleteBehavior.Cascade); // Optional: Cascade delete

            // Configure one-to-many relationship between Laundry and Order
            modelBuilder.Entity<Laundry>()
                .HasMany(l => l.Orders)
                .WithOne(o => o.Laundry)
                .HasForeignKey(o => o.LaundryId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure one-to-one relationship between Order and Payment
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Payment)
                .WithOne(p => p.Order)
                .HasForeignKey<Payment>(p => p.OrderId)
                .OnDelete(DeleteBehavior.Cascade); // Optional: Cascade delete

            // Configure one-to-many relationship between Customer and Order
            modelBuilder.Entity<Customer>()
                .HasMany(c => c.Orders)
                .WithOne(o => o.Customer)
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure one-to-many relationship between Laundry and Feedback
            modelBuilder.Entity<Laundry>()
                .HasMany(l => l.Feedbacks)
                .WithOne(f => f.Laundry)
                .HasForeignKey(f => f.LaundryId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure one-to-many relationship between Customer and Feedback
            modelBuilder.Entity<Customer>()
                .HasMany(c => c.Feedbacks)
                .WithOne(f => f.Customer)
                .HasForeignKey(f => f.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure one-to-many relationship between UserGroup and User
            modelBuilder.Entity<UserGroup>()
                .HasMany(ug => ug.Users)
                .WithOne(u => u.UserGroup)
                .HasForeignKey(u => u.UserGroupId)
                .OnDelete(DeleteBehavior.Cascade); // Optional: Cascade delete

            // Configure one-to-many relationship between User and Order
            modelBuilder.Entity<User>()
                .HasMany(u => u.Orders)
                .WithOne(o => o.User)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Optional: Cascade delete

            // Configure many-to-many relationship between Item and Service
            modelBuilder.Entity<ItemService>()
                .HasKey(itemService => itemService.ItemServiceId); // Primary key for the join table

            modelBuilder.Entity<ItemService>()
                .HasOne(itemService => itemService.Item)
                .WithMany(item => item.ItemServices)
                .HasForeignKey(itemService => itemService.ItemId);

            modelBuilder.Entity<ItemService>()
                .HasOne(itemService => itemService.Service)
                .WithMany(service => service.ItemServices)
                .HasForeignKey(itemService => itemService.ServiceId);

            // Configure many-to-many relationship between Order and Item
            modelBuilder.Entity<OrderItem>()
                .HasKey(oi => oi.OrderItemId); // Primary key for the join table

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Item)
                .WithMany(i => i.OrderItems)
                .HasForeignKey(oi => oi.ItemId);


            // Configure many-to-many relationship between Customer and Laundry
            modelBuilder.Entity<CustomerLaundry>()
                .HasKey(cl => cl.CustomerLaundryId); // Primary key for the join table

            modelBuilder.Entity<CustomerLaundry>()
                .HasOne(cl => cl.Customer)
                .WithMany(c => c.CustomerLaundries)
                .HasForeignKey(cl => cl.CustomerId);

            modelBuilder.Entity<CustomerLaundry>()
                .HasOne(cl => cl.Laundry)
                .WithMany(l => l.CustomerLaundries)
                .HasForeignKey(cl => cl.LaundryId);

            // Configure many-to-many relationship between User and Laundry
            modelBuilder.Entity<UserLaundry>()
                .HasKey(ul => ul.UserLaundryId); // Primary key for the join table

            modelBuilder.Entity<UserLaundry>()
                .HasOne(ul => ul.User)
                .WithMany(u => u.UserLaundries)
                .HasForeignKey(ul => ul.UserId);

            modelBuilder.Entity<UserLaundry>()
                .HasOne(ul => ul.Laundry)
                .WithMany(l => l.UserLaundries)
                .HasForeignKey(ul => ul.LaundryId);

            // Configure many-to-many relationship between User and Customer
            modelBuilder.Entity<UserCustomer>()
                .HasKey(uc => uc.UserCustomerId); // Primary key for the join table

            modelBuilder.Entity<UserCustomer>()
                .HasOne(uc => uc.User)
                .WithMany(u => u.UserCustomers)
                .HasForeignKey(uc => uc.UserId);

            modelBuilder.Entity<UserCustomer>()
                .HasOne(uc => uc.Customer)
                .WithMany(c => c.UserCustomers)
                .HasForeignKey(uc => uc.CustomerId);


            // Configure many-to-many relationship between UserGroup and Privilege
            modelBuilder.Entity<UserGroupPrivilege>()
                .HasKey(ugp => ugp.UserGroupPrivilegeId); // Primary key for the join table

            modelBuilder.Entity<UserGroupPrivilege>()
                .HasOne(ugp => ugp.UserGroup)
                .WithMany(ug => ug.UserGroupPrivileges)
                .HasForeignKey(ugp => ugp.UserGroupId);

            modelBuilder.Entity<UserGroupPrivilege>()
                .HasOne(ugp => ugp.Privilege)
                .WithMany(p => p.UserGroupPrivileges)
                .HasForeignKey(ugp => ugp.PrivilegeId);

        }
    }
}
