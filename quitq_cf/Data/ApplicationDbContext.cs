using Microsoft.EntityFrameworkCore;
using quitq_cf.Models;

namespace quitq_cf.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext() { }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public virtual DbSet<Admin> Admins { get; set; } = null!;
        public virtual DbSet<AdminReport> AdminReports { get; set; } = null!;
        public virtual DbSet<Cart> Carts { get; set; } = null!;
        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<Customer> Customers { get; set; } = null!;
        public virtual DbSet<Order> Orders { get; set; } = null!;
        public virtual DbSet<OrderDetail> OrderDetails { get; set; } = null!;
        public virtual DbSet<OrderStatus> OrderStatuses { get; set; } = null!;
        public virtual DbSet<Payment> Payments { get; set; } = null!;
        public virtual DbSet<Product> Products { get; set; } = null!;
        public virtual DbSet<Seller> Sellers { get; set; } = null!;
        public virtual DbSet<StockInfo> StockInfos { get; set; } = null!;
        public virtual DbSet<SubCategory> SubCategories { get; set; } = null!;
        public DbSet<RevenueReportDTO> RevenueReports { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=ARYAN ;Database=qqdb;Integrated Security=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<RevenueReportDTO>().HasNoKey();

            modelBuilder.Entity<Admin>(entity =>
            {
                entity.HasKey(e => e.UserId)
                    .HasName("PK__Admin__CB9A1CDFB9D1612C");

                entity.ToTable("Admin");

                entity.HasIndex(e => e.Email, "UQ_Admin_Email")
                    .IsUnique();

                entity.HasIndex(e => e.PhoneNumber, "UQ_Admin_PhoneNumber")
                    .IsUnique();

                entity.HasIndex(e => e.UserName, "UQ_Admin_Username")
                    .IsUnique();

                entity.Property(e => e.UserId)
                    .HasMaxLength(100)
                    .HasColumnName("userID");

                entity.Property(e => e.DateOfBirth).HasColumnType("date");

                entity.Property(e => e.Email).HasMaxLength(100);

                entity.Property(e => e.Password).HasMaxLength(200);

                entity.Property(e => e.PhoneNumber).HasMaxLength(15);

                entity.Property(e => e.Role)
                    .HasMaxLength(50)
                    .HasDefaultValueSql("('Admin')");

                entity.Property(e => e.UserName).HasMaxLength(255);
            });

            modelBuilder.Entity<AdminReport>(entity =>
            {
                entity.HasKey(e => e.ReportId)
                    .HasName("PK__admin_re__779B7C580A6C5416");

                entity.ToTable("admin_reports");

                entity.Property(e => e.ReportId).HasColumnName("report_id");

                entity.Property(e => e.AdminId)
                    .HasMaxLength(100)
                    .HasColumnName("admin_id");

                entity.Property(e => e.Content).HasColumnName("content");

                entity.Property(e => e.ReportDate)
                    .HasColumnType("datetime")
                    .HasColumnName("report_date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ReportType)
                    .HasMaxLength(100)
                    .HasColumnName("report_type");

                entity.HasOne(d => d.Admin)
                    .WithMany(p => p.AdminReports)
                    .HasForeignKey(d => d.AdminId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_AdminReports_Admins");
            });

            modelBuilder.Entity<Cart>(entity =>
            {
                entity.ToTable("cart");

                entity.Property(e => e.CartId).HasColumnName("cart_id");

                entity.Property(e => e.ProductId).HasColumnName("product_id");

                entity.Property(e => e.Quantity).HasColumnName("quantity");

                entity.Property(e => e.UserId)
                    .HasMaxLength(100)
                    .HasColumnName("user_id");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.Carts)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Cart_Products");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Carts)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Cart_Users");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("categories");

                entity.HasIndex(e => e.CategoryName, "UQ__categori__5189E255DC054B73")
                    .IsUnique();

                entity.Property(e => e.CategoryId).HasColumnName("category_id");

                entity.Property(e => e.CategoryName)
                    .HasMaxLength(100)
                    .HasColumnName("category_name");
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.UserId)
                    .HasName("PK__Customer__CB9A1CDF0D57F7E5");

                entity.ToTable("Customer");

                entity.HasIndex(e => e.Email, "UQ_Customer_Email")
                    .IsUnique();

                entity.HasIndex(e => e.PhoneNumber, "UQ_Customer_PhoneNumber")
                    .IsUnique();

                entity.HasIndex(e => e.UserName, "UQ_Customer_Username")
                    .IsUnique();

                entity.Property(e => e.UserId)
                    .HasMaxLength(100)
                    .HasColumnName("userID");

                entity.Property(e => e.DateOfBirth).HasColumnType("date");

                entity.Property(e => e.Email).HasMaxLength(100);

                entity.Property(e => e.Password).HasMaxLength(200);

                entity.Property(e => e.PhoneNumber).HasMaxLength(15);

                entity.Property(e => e.Role)
                    .HasMaxLength(50)
                    .HasDefaultValueSql("('Customer')");

                entity.Property(e => e.UserName)
                    .HasMaxLength(255)
                    .HasColumnName("userName");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("orders");

                entity.Property(e => e.OrderId).HasColumnName("order_id");

                entity.Property(e => e.ShippingAddress)
                    .HasMaxLength(255)
                    .HasColumnName("shipping_address");

                

                entity.Property(e => e.StatusId).HasColumnName("status_id");

                entity.Property(e => e.TotalAmount)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("total_amount");

                entity.Property(e => e.UserId)
                    .HasMaxLength(100)
                    .HasColumnName("user_id");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_Orders_OrderStatus");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Orders_Users");
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.ToTable("order_details");

                entity.Property(e => e.OrderdetailId).HasColumnName("orderdetail_id");

                entity.Property(e => e.OrderId).HasColumnName("order_id");

                entity.Property(e => e.Price)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("price");

                entity.Property(e => e.ProductId).HasColumnName("product_id");

                entity.Property(e => e.Quantity).HasColumnName("quantity");

                entity.Property(e => e.Total)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("total");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_OrderDetails_Orders");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_OrderDetails_Products");
            });

            modelBuilder.Entity<OrderStatus>(entity =>
            {
                entity.HasKey(e => e.StatusId)
                    .HasName("PK__order_st__3683B531443775E3");

                entity.ToTable("order_status");

                entity.HasIndex(e => e.StatusName, "UQ__order_st__501B375341FC5B58")
                    .IsUnique();

                entity.Property(e => e.StatusId).HasColumnName("status_id");

                entity.Property(e => e.StatusName)
                    .HasMaxLength(50)
                    .HasColumnName("status_name");
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.ToTable("payments");

                entity.HasIndex(e => e.TransactionId, "UQ__payments__85C600AEB41D296D")
                    .IsUnique();

                entity.Property(e => e.PaymentId)
                    .HasMaxLength(50)
                    .HasColumnName("payment_id");

                entity.Property(e => e.Amount)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("amount");

                entity.Property(e => e.OrderId).HasColumnName("order_id");

                entity.Property(e => e.PaymentDate)
                    .HasColumnType("datetime")
                    .HasColumnName("payment_date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.PaymentMethod)
                    .HasMaxLength(50)
                    .HasColumnName("payment_method");

                entity.Property(e => e.PaymentStatus)
                    .HasMaxLength(50)
                    .HasColumnName("payment_status");

                entity.Property(e => e.TransactionId)
                    .HasMaxLength(100)
                    .HasColumnName("transaction_id");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.Payments)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Payments_Orders");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("products");

                entity.Property(e => e.ProductId).HasColumnName("product_id");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.ImageUrl)
                    .HasMaxLength(255)
                    .HasColumnName("image_url");

                entity.Property(e => e.Price)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("price");

                entity.Property(e => e.ProductName)
                    .HasMaxLength(200)
                    .HasColumnName("product_name");

                entity.Property(e => e.SellerId)
                    .HasMaxLength(100)
                    .HasColumnName("seller_id");

                entity.Property(e => e.Stock).HasColumnName("stock");

                entity.Property(e => e.SubcategoryId).HasColumnName("subcategory_id");

                entity.HasOne(d => d.Seller)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.SellerId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_Products_Sellers");

                entity.HasOne(d => d.Subcategory)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.SubcategoryId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Products_SubCategories");
            });

            modelBuilder.Entity<Seller>(entity =>
            {
                entity.HasKey(e => e.UserId)
                    .HasName("PK__Seller__CB9A1CDFFCAFC9E3");

                entity.ToTable("Seller");

                entity.HasIndex(e => e.Email, "UQ_Seller_Email")
                    .IsUnique();

                entity.HasIndex(e => e.PhoneNumber, "UQ_Seller_PhoneNumber")
                    .IsUnique();

                entity.HasIndex(e => e.UserName, "UQ_seller_Username")
                    .IsUnique();

                entity.Property(e => e.UserId)
                    .HasMaxLength(100)
                    .HasColumnName("userID");

                entity.Property(e => e.DateOfBirth).HasColumnType("date");

                entity.Property(e => e.Email).HasMaxLength(100);

                entity.Property(e => e.Password).HasMaxLength(200);

                entity.Property(e => e.PhoneNumber).HasMaxLength(15);

                entity.Property(e => e.Role)
                    .HasMaxLength(50)
                    .HasDefaultValueSql("('Seller')");

                entity.Property(e => e.UserName).HasMaxLength(255);
            });

            modelBuilder.Entity<StockInfo>(entity =>
            {
                entity.HasKey(e => e.StockId)
                    .HasName("PK__stock_in__E8666862A113FD8E");

                entity.ToTable("stock_info");

                entity.Property(e => e.StockId).HasColumnName("stock_id");

                entity.Property(e => e.ProductId).HasColumnName("product_id");

                entity.Property(e => e.Quantity).HasColumnName("quantity");

                entity.Property(e => e.SellerLocation)
                    .HasMaxLength(100)
                    .HasColumnName("seller_location");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.StockInfos)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_StockInfo_Products");
            });

            modelBuilder.Entity<SubCategory>(entity =>
            {
                entity.ToTable("sub_categories");

                entity.HasIndex(e => e.SubcategoryName, "UQ__sub_cate__5B737073EA0050F7")
                    .IsUnique();

                entity.Property(e => e.SubcategoryId).HasColumnName("subcategory_id");

                entity.Property(e => e.CategoryId).HasColumnName("category_id");

                entity.Property(e => e.SubcategoryName)
                    .HasMaxLength(100)
                    .HasColumnName("subcategory_name");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.SubCategories)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_SubCategories_Categories");
            });

            //OnModelCreatingPartial(modelBuilder);
        }

        //partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
