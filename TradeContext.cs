using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WebApplication3
{
    public partial class TradeContext : DbContext
    {
        public TradeContext()
        {
        }
        public TradeContext(DbContextOptions<TradeContext> options) : base(options)
        {
        }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderProduct> OrderProducts { get; set; }
        public virtual DbSet<PickupPoint> PickupPoints { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
            optionsBuilder.UseSqlServer("Server=MSI\\SQLEXPRESS;Database=Trade;Trusted_Connection=True;TrustServerCertificate=True;Encrypt=False");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.OrderId).HasName("PK__Order__C3905BAFA61348B3");

                entity.ToTable("Order");

                entity.Property(e => e.OrderId).HasColumnName("OrderID");
                entity.Property(e => e.Code)
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.DateOrders).HasColumnType("date");
                entity.Property(e => e.NameClient)
                    .HasMaxLength(100)
                    .IsUnicode(false);
                entity.Property(e => e.OrderDeliveryDate).HasColumnType("datetime");

                entity.HasOne(d => d.OrderPickupPointNavigation).WithMany(p => p.Orders)
                    .HasForeignKey(d => d.OrderPickupPoint)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Order_PickupPoint");
            });

            modelBuilder.Entity<OrderProduct>(entity =>
            {
                entity.HasKey(e => new { e.OrderId, e.ProductArticleNumber });

                entity.ToTable("OrderProduct");

                entity.Property(e => e.OrderId).HasColumnName("OrderID");
                entity.Property(e => e.ProductArticleNumber).HasMaxLength(100);
                entity.Property(e => e.Count)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Order).WithMany(p => p.OrderProducts)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__OrderProd__Order__403A8C7D");

                entity.HasOne(d => d.ProductArticleNumberNavigation).WithMany(p => p.OrderProducts)
                    .HasForeignKey(d => d.ProductArticleNumber)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__OrderProd__Produ__412EB0B6");
            });

            modelBuilder.Entity<PickupPoint>(entity =>
            {
                entity.ToTable("PickupPoint");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");
                entity.Property(e => e.Address)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("address");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.ProductArticleNumber).HasName("PK__Product__2EA7DCD5712109A4");

                entity.ToTable("Product");

                entity.Property(e => e.ProductArticleNumber).HasMaxLength(100);
                entity.Property(e => e.CountInPack).HasMaxLength(50);
                entity.Property(e => e.MaxDiscountAmount).HasMaxLength(50);
                entity.Property(e => e.MinCount).HasMaxLength(50);
                entity.Property(e => e.ProductCost).HasColumnType("decimal(19, 4)");
                entity.Property(e => e.ProductPhoto).HasMaxLength(50);
                entity.Property(e => e.Supplier).HasMaxLength(50);
                entity.Property(e => e.Unit).HasMaxLength(50);
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.RoleId).HasName("PK__Role__8AFACE3A36F7B3D7");

                entity.ToTable("Role");

                entity.Property(e => e.RoleId).HasColumnName("RoleID");
                entity.Property(e => e.RoleName).HasMaxLength(100);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId).HasName("PK__User__1788CCACA1B0DA9A");

                entity.ToTable("User");

                entity.Property(e => e.UserId).HasColumnName("UserID");
                entity.Property(e => e.UserName).HasMaxLength(100);
                entity.Property(e => e.UserPatronymic).HasMaxLength(100);
                entity.Property(e => e.UserSurname).HasMaxLength(100);

                entity.HasOne(d => d.UserRoleNavigation).WithMany(p => p.Users)
                    .HasForeignKey(d => d.UserRole)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__User__UserRole__398D8EEE");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
