using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace FreshlyBackendNew.ScaffoldedModels;

public partial class FreshlydbContext : DbContext
{
    public FreshlydbContext()
    {
    }

    public FreshlydbContext(DbContextOptions<FreshlydbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Address> Addresses { get; set; }

    public virtual DbSet<Contact> Contacts { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Driver> Drivers { get; set; }

    public virtual DbSet<Drivernote> Drivernotes { get; set; }

    public virtual DbSet<Efmigrationshistory> Efmigrationshistories { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<Item> Items { get; set; }

    public virtual DbSet<Itemcategory> Itemcategories { get; set; }

    public virtual DbSet<Laundry> Laundries { get; set; }

    public virtual DbSet<Laundryitemservice> Laundryitemservices { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<Orderdetail> Orderdetails { get; set; }

    public virtual DbSet<Owner> Owners { get; set; }

    public virtual DbSet<Service> Services { get; set; }

    public virtual DbSet<Session> Sessions { get; set; }

    public virtual DbSet<Status> Statuses { get; set; }

    public virtual DbSet<Temporaryorder> Temporaryorders { get; set; }

    public virtual DbSet<Temporaryorderdetail> Temporaryorderdetails { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=freshly-mysql-azure.mysql.database.azure.com;database=freshlydb;user=LasiniPallewaththa;password=Freshly2002", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.41-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Address>(entity =>
        {
            entity.HasKey(e => e.AddressId).HasName("PRIMARY");

            entity.ToTable("addresses");

            entity.Property(e => e.AddressId)
                .UseCollation("ascii_general_ci")
                .HasCharSet("ascii");
        });

        modelBuilder.Entity<Contact>(entity =>
        {
            entity.HasKey(e => e.ContactId).HasName("PRIMARY");

            entity.ToTable("contacts");

            entity.Property(e => e.ContactId)
                .UseCollation("ascii_general_ci")
                .HasCharSet("ascii");
            entity.Property(e => e.UserId)
                .UseCollation("ascii_general_ci")
                .HasCharSet("ascii");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("PRIMARY");

            entity.ToTable("customers");

            entity.HasIndex(e => e.AddressId, "IX_Customers_AddressId");

            entity.Property(e => e.CustomerId)
                .UseCollation("ascii_general_ci")
                .HasCharSet("ascii");
            entity.Property(e => e.AddressId)
                .UseCollation("ascii_general_ci")
                .HasCharSet("ascii");

            entity.HasOne(d => d.Address).WithMany(p => p.Customers)
                .HasForeignKey(d => d.AddressId)
                .HasConstraintName("FK_Customers_Addresses_AddressId");
        });

        modelBuilder.Entity<Driver>(entity =>
        {
            entity.HasKey(e => e.DriverId).HasName("PRIMARY");

            entity.ToTable("drivers");

            entity.HasIndex(e => e.AddressId, "IX_Drivers_AddressId");

            entity.Property(e => e.DriverId)
                .UseCollation("ascii_general_ci")
                .HasCharSet("ascii");
            entity.Property(e => e.AddressId)
                .UseCollation("ascii_general_ci")
                .HasCharSet("ascii");

            entity.HasOne(d => d.Address).WithMany(p => p.Drivers)
                .HasForeignKey(d => d.AddressId)
                .HasConstraintName("FK_Drivers_Addresses_AddressId");
        });

        modelBuilder.Entity<Drivernote>(entity =>
        {
            entity.HasKey(e => e.NoteId).HasName("PRIMARY");

            entity.ToTable("drivernotes");

            entity.HasIndex(e => e.DriverId, "IX_DriverNotes_DriverId");

            entity.HasIndex(e => e.OrderId, "IX_DriverNotes_OrderId");

            entity.Property(e => e.NoteId)
                .UseCollation("ascii_general_ci")
                .HasCharSet("ascii");
            entity.Property(e => e.DriverId)
                .UseCollation("ascii_general_ci")
                .HasCharSet("ascii");
            entity.Property(e => e.OrderId)
                .UseCollation("ascii_general_ci")
                .HasCharSet("ascii");

            entity.HasOne(d => d.Driver).WithMany(p => p.Drivernotes)
                .HasForeignKey(d => d.DriverId)
                .HasConstraintName("FK_DriverNotes_Drivers_DriverId");

            entity.HasOne(d => d.Order).WithMany(p => p.Drivernotes)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK_DriverNotes_Orders_OrderId");
        });

        modelBuilder.Entity<Efmigrationshistory>(entity =>
        {
            entity.HasKey(e => e.MigrationId).HasName("PRIMARY");

            entity.ToTable("__efmigrationshistory");

            entity.Property(e => e.MigrationId).HasMaxLength(150);
            entity.Property(e => e.ProductVersion).HasMaxLength(32);
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.FeedbackId).HasName("PRIMARY");

            entity.ToTable("feedbacks");

            entity.HasIndex(e => e.OrderId, "IX_Feedbacks_OrderId");

            entity.Property(e => e.FeedbackId)
                .UseCollation("ascii_general_ci")
                .HasCharSet("ascii");
            entity.Property(e => e.OrderId)
                .UseCollation("ascii_general_ci")
                .HasCharSet("ascii");

            entity.HasOne(d => d.Order).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK_Feedbacks_Orders_OrderId");
        });

        modelBuilder.Entity<Item>(entity =>
        {
            entity.HasKey(e => e.ItemId).HasName("PRIMARY");

            entity.ToTable("items");

            entity.HasIndex(e => e.CategoryId, "IX_Items_CategoryId");

            entity.Property(e => e.ItemId)
                .UseCollation("ascii_general_ci")
                .HasCharSet("ascii");
            entity.Property(e => e.CategoryId)
                .UseCollation("ascii_general_ci")
                .HasCharSet("ascii");

            entity.HasOne(d => d.Category).WithMany(p => p.Items)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK_Items_ItemCategories_CategoryId");
        });

        modelBuilder.Entity<Itemcategory>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PRIMARY");

            entity.ToTable("itemcategories");

            entity.Property(e => e.CategoryId)
                .UseCollation("ascii_general_ci")
                .HasCharSet("ascii");
        });

        modelBuilder.Entity<Laundry>(entity =>
        {
            entity.HasKey(e => e.LaundryId).HasName("PRIMARY");

            entity.ToTable("laundries");

            entity.HasIndex(e => e.AddressId, "IX_Laundries_AddressId");

            entity.HasIndex(e => e.OwnerId, "IX_Laundries_OwnerId");

            entity.Property(e => e.LaundryId)
                .UseCollation("ascii_general_ci")
                .HasCharSet("ascii");
            entity.Property(e => e.AddressId)
                .UseCollation("ascii_general_ci")
                .HasCharSet("ascii");
            entity.Property(e => e.OwnerId)
                .UseCollation("ascii_general_ci")
                .HasCharSet("ascii");

            entity.HasOne(d => d.Address).WithMany(p => p.Laundries)
                .HasForeignKey(d => d.AddressId)
                .HasConstraintName("FK_Laundries_Addresses_AddressId");

            entity.HasOne(d => d.Owner).WithMany(p => p.Laundries)
                .HasForeignKey(d => d.OwnerId)
                .HasConstraintName("FK_Laundries_Owners_OwnerId");
        });

        modelBuilder.Entity<Laundryitemservice>(entity =>
        {
            entity.HasKey(e => new { e.LaundryId, e.ItemId, e.ServiceId })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0, 0 });

            entity.ToTable("laundryitemservices");

            entity.HasIndex(e => e.ItemId, "IX_LaundryItemServices_ItemId");

            entity.HasIndex(e => e.ServiceId, "IX_LaundryItemServices_ServiceId");

            entity.Property(e => e.LaundryId)
                .UseCollation("ascii_general_ci")
                .HasCharSet("ascii");
            entity.Property(e => e.ItemId)
                .UseCollation("ascii_general_ci")
                .HasCharSet("ascii");
            entity.Property(e => e.ServiceId)
                .UseCollation("ascii_general_ci")
                .HasCharSet("ascii");

            entity.HasOne(d => d.Item).WithMany(p => p.Laundryitemservices)
                .HasForeignKey(d => d.ItemId)
                .HasConstraintName("FK_LaundryItemServices_Items_ItemId");

            entity.HasOne(d => d.Laundry).WithMany(p => p.Laundryitemservices)
                .HasForeignKey(d => d.LaundryId)
                .HasConstraintName("FK_LaundryItemServices_Laundries_LaundryId");

            entity.HasOne(d => d.Service).WithMany(p => p.Laundryitemservices)
                .HasForeignKey(d => d.ServiceId)
                .HasConstraintName("FK_LaundryItemServices_Services_ServiceId");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PRIMARY");

            entity.ToTable("orders");

            entity.HasIndex(e => e.CustomerId, "IX_Orders_CustomerId");

            entity.HasIndex(e => e.LaundryId, "IX_Orders_LaundryId");

            entity.HasIndex(e => e.StatusId, "IX_Orders_StatusId");

            entity.Property(e => e.OrderId)
                .UseCollation("ascii_general_ci")
                .HasCharSet("ascii");
            entity.Property(e => e.CustomerId)
                .UseCollation("ascii_general_ci")
                .HasCharSet("ascii");
            entity.Property(e => e.LaundryId)
                .UseCollation("ascii_general_ci")
                .HasCharSet("ascii");
            entity.Property(e => e.PickupAt).HasMaxLength(6);
            entity.Property(e => e.PlacedAt).HasMaxLength(6);
            entity.Property(e => e.StatusId)
                .UseCollation("ascii_general_ci")
                .HasCharSet("ascii");

            entity.HasOne(d => d.Customer).WithMany(p => p.Orders)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK_Orders_Customers_CustomerId");

            entity.HasOne(d => d.Laundry).WithMany(p => p.Orders)
                .HasForeignKey(d => d.LaundryId)
                .HasConstraintName("FK_Orders_Laundries_LaundryId");

            entity.HasOne(d => d.Status).WithMany(p => p.Orders)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK_Orders_Statuses_StatusId");
        });

        modelBuilder.Entity<Orderdetail>(entity =>
        {
            entity.HasKey(e => new { e.OrderId, e.ItemId, e.ServiceId })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0, 0 });

            entity.ToTable("orderdetails");

            entity.HasIndex(e => e.ItemId, "IX_OrderDetails_ItemId");

            entity.HasIndex(e => e.ServiceId, "IX_OrderDetails_ServiceId");

            entity.Property(e => e.OrderId)
                .UseCollation("ascii_general_ci")
                .HasCharSet("ascii");
            entity.Property(e => e.ItemId)
                .UseCollation("ascii_general_ci")
                .HasCharSet("ascii");
            entity.Property(e => e.ServiceId)
                .UseCollation("ascii_general_ci")
                .HasCharSet("ascii");

            entity.HasOne(d => d.Item).WithMany(p => p.Orderdetails)
                .HasForeignKey(d => d.ItemId)
                .HasConstraintName("FK_OrderDetails_Items_ItemId");

            entity.HasOne(d => d.Order).WithMany(p => p.Orderdetails)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK_OrderDetails_Orders_OrderId");

            entity.HasOne(d => d.Service).WithMany(p => p.Orderdetails)
                .HasForeignKey(d => d.ServiceId)
                .HasConstraintName("FK_OrderDetails_Services_ServiceId");
        });

        modelBuilder.Entity<Owner>(entity =>
        {
            entity.HasKey(e => e.OwnerId).HasName("PRIMARY");

            entity.ToTable("owners");

            entity.HasIndex(e => e.AddressId, "IX_Owners_AddressId");

            entity.Property(e => e.OwnerId)
                .UseCollation("ascii_general_ci")
                .HasCharSet("ascii");
            entity.Property(e => e.AddressId)
                .UseCollation("ascii_general_ci")
                .HasCharSet("ascii");

            entity.HasOne(d => d.Address).WithMany(p => p.Owners)
                .HasForeignKey(d => d.AddressId)
                .HasConstraintName("FK_Owners_Addresses_AddressId");
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.ServiceId).HasName("PRIMARY");

            entity.ToTable("services");

            entity.Property(e => e.ServiceId)
                .UseCollation("ascii_general_ci")
                .HasCharSet("ascii");
        });

        modelBuilder.Entity<Session>(entity =>
        {
            entity.HasKey(e => e.SessionId).HasName("PRIMARY");

            entity.ToTable("sessions");

            entity.HasIndex(e => e.UserId, "IX_Sessions_UserId");

            entity.Property(e => e.SessionId)
                .UseCollation("ascii_general_ci")
                .HasCharSet("ascii");
            entity.Property(e => e.CreatedAt).HasMaxLength(6);
            entity.Property(e => e.ExpiredAt).HasMaxLength(6);
            entity.Property(e => e.UserId)
                .UseCollation("ascii_general_ci")
                .HasCharSet("ascii");

            entity.HasOne(d => d.User).WithMany(p => p.Sessions)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Sessions_Customers_UserId");

            entity.HasOne(d => d.UserNavigation).WithMany(p => p.Sessions)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Sessions_Drivers_UserId");

            entity.HasOne(d => d.User1).WithMany(p => p.Sessions)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Sessions_Laundries_UserId");

            entity.HasOne(d => d.User2).WithMany(p => p.Sessions)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Sessions_Owners_UserId");
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("PRIMARY");

            entity.ToTable("statuses");

            entity.Property(e => e.StatusId)
                .HasColumnName("StatusID")
                .UseCollation("ascii_general_ci")
                .HasCharSet("ascii");
        });

        modelBuilder.Entity<Temporaryorder>(entity =>
        {
            entity.HasKey(e => e.TemporaryOrderId).HasName("PRIMARY");

            entity.ToTable("temporaryorders");

            entity.HasIndex(e => e.CustomerId, "IX_TemporaryOrders_CustomerId");

            entity.HasIndex(e => e.LaundryId, "IX_TemporaryOrders_LaundryId");

            entity.HasIndex(e => e.StatusId, "IX_TemporaryOrders_StatusId");

            entity.Property(e => e.TemporaryOrderId)
                .UseCollation("ascii_general_ci")
                .HasCharSet("ascii");
            entity.Property(e => e.CustomerId)
                .UseCollation("ascii_general_ci")
                .HasCharSet("ascii");
            entity.Property(e => e.LaundryId)
                .UseCollation("ascii_general_ci")
                .HasCharSet("ascii");
            entity.Property(e => e.PickupAt).HasMaxLength(6);
            entity.Property(e => e.PlacedAt).HasMaxLength(6);
            entity.Property(e => e.StatusId)
                .UseCollation("ascii_general_ci")
                .HasCharSet("ascii");

            entity.HasOne(d => d.Customer).WithMany(p => p.Temporaryorders)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK_TemporaryOrders_Customers_CustomerId");

            entity.HasOne(d => d.Laundry).WithMany(p => p.Temporaryorders)
                .HasForeignKey(d => d.LaundryId)
                .HasConstraintName("FK_TemporaryOrders_Laundries_LaundryId");

            entity.HasOne(d => d.Status).WithMany(p => p.Temporaryorders)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK_TemporaryOrders_Statuses_StatusId");
        });

        modelBuilder.Entity<Temporaryorderdetail>(entity =>
        {
            entity.HasKey(e => new { e.TemporaryOrderId, e.ItemId, e.ServiceId })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0, 0 });

            entity.ToTable("temporaryorderdetails");

            entity.HasIndex(e => e.ItemId, "IX_TemporaryOrderDetails_ItemId");

            entity.HasIndex(e => e.ServiceId, "IX_TemporaryOrderDetails_ServiceId");

            entity.Property(e => e.TemporaryOrderId)
                .UseCollation("ascii_general_ci")
                .HasCharSet("ascii");
            entity.Property(e => e.ItemId)
                .UseCollation("ascii_general_ci")
                .HasCharSet("ascii");
            entity.Property(e => e.ServiceId)
                .UseCollation("ascii_general_ci")
                .HasCharSet("ascii");

            entity.HasOne(d => d.Item).WithMany(p => p.Temporaryorderdetails)
                .HasForeignKey(d => d.ItemId)
                .HasConstraintName("FK_TemporaryOrderDetails_Items_ItemId");

            entity.HasOne(d => d.Service).WithMany(p => p.Temporaryorderdetails)
                .HasForeignKey(d => d.ServiceId)
                .HasConstraintName("FK_TemporaryOrderDetails_Services_ServiceId");

            entity.HasOne(d => d.TemporaryOrder).WithMany(p => p.Temporaryorderdetails)
                .HasForeignKey(d => d.TemporaryOrderId)
                .HasConstraintName("FK_TemporaryOrderDetails_TemporaryOrders_TemporaryOrderId");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
