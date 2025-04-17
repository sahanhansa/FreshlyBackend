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




        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Call the base method first

            // Ensures a new Guid is generated when adding a record
            modelBuilder.Entity<Customer>()
                .Property(c => c.CustomerId)
                .ValueGeneratedOnAdd();
        }
    }
}
