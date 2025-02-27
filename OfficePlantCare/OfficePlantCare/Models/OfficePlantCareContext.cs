using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace OfficePlantCare.Models;

public partial class OfficePlantCareContext : DbContext
{
    public OfficePlantCareContext()
    {
    }

    public OfficePlantCareContext(DbContextOptions<OfficePlantCareContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<Banner> Banners { get; set; }

    public virtual DbSet<CareSchedule> CareSchedules { get; set; }

    public virtual DbSet<CareScheduleInventory> CareScheduleInventories { get; set; }

    public virtual DbSet<Contact> Contacts { get; set; }

    public virtual DbSet<Contract> Contracts { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<InventoryItem> InventoryItems { get; set; }

    public virtual DbSet<News> News { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<Partner> Partners { get; set; }

    public virtual DbSet<PaymentMethod> PaymentMethods { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Service> Services { get; set; }

    public virtual DbSet<ServiceCategory> ServiceCategories { get; set; }

    public virtual DbSet<ServiceDescription> ServiceDescriptions { get; set; }

    public virtual DbSet<ServiceInventory> ServiceInventories { get; set; }

    public virtual DbSet<ServicePrice> ServicePrices { get; set; }

    public virtual DbSet<ServiceRequest> ServiceRequests { get; set; }

    public virtual DbSet<Staff> Staffs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=Ruby;Database=OfficePlantCare;uid=sa;pwd=123456;MultipleActiveResultSets=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasKey(e => e.AdminId).HasName("PK__Admins__719FE488051162DB");

            entity.HasIndex(e => e.Email, "UQ__Admins__A9D10534530BD1CA").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Admins__A9D105347FB7635E").IsUnique();

            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Hoạt động");
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.Username).HasMaxLength(255);

            entity.HasOne(d => d.Role).WithMany(p => p.Admins)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Admins__RoleId__114A936A");
        });

        modelBuilder.Entity<Banner>(entity =>
        {
            entity.HasKey(e => e.BannerId).HasName("PK__Banners__32E86AD1F17C6FCC");

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Hiện");
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<CareSchedule>(entity =>
        {
            entity.HasKey(e => e.ScheduleId).HasName("PK__CareSche__9C8A5B4952457F55");

            entity.Property(e => e.Duration).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.ScheduledDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Chưa thực hiện");

            entity.HasOne(d => d.Contract).WithMany(p => p.CareSchedules)
                .HasForeignKey(d => d.ContractId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CareSched__Contr__14270015");

            entity.HasOne(d => d.Staff).WithMany(p => p.CareSchedules)
                .HasForeignKey(d => d.StaffId)
                .HasConstraintName("FK__CareSched__Staff__151B244E");
        });

        modelBuilder.Entity<CareScheduleInventory>(entity =>
        {
            entity.HasKey(e => e.CareScheduleInventoryId).HasName("PK__CareSche__D1DD1ADC402B7994");

            entity.ToTable("CareScheduleInventory");

            entity.HasOne(d => d.CareSchedule).WithMany(p => p.CareScheduleInventories)
                .HasForeignKey(d => d.CareScheduleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CareSched__CareS__123EB7A3");

            entity.HasOne(d => d.Item).WithMany(p => p.CareScheduleInventories)
                .HasForeignKey(d => d.ItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CareSched__ItemI__1332DBDC");
        });

        modelBuilder.Entity<Contact>(entity =>
        {
            entity.HasKey(e => e.ContactId).HasName("PK__Contacts__5C66259BA146EF9A");

            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Chưa xử lý");
        });

        modelBuilder.Entity<Contract>(entity =>
        {
            entity.HasKey(e => e.ContractId).HasName("PK__Contract__C90D346985090AA1");

            entity.ToTable(tb => tb.HasTrigger("trg_UpdateRemainingAmount"));

            entity.Property(e => e.ContractCode)
                .HasMaxLength(6)
                .HasComputedColumnSql("('HD'+right('0000'+CONVERT([nvarchar],[ContractId]),(4)))", true);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DurationUnit)
                .HasMaxLength(10)
                .HasDefaultValue("Month");
            entity.Property(e => e.EndDate).HasComputedColumnSql("(case when [DurationUnit]='Month' then dateadd(month,[Duration],[StartDate]) when [DurationUnit]='Year' then dateadd(year,[Duration],[StartDate])  end)", true);
            entity.Property(e => e.FixedPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PaidAmount)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.RemainingAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Đang hiệu lực");
            entity.Property(e => e.TotalAmount)
                .HasComputedColumnSql("([FixedPrice]*[Duration])", true)
                .HasColumnType("decimal(29, 2)");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Customer).WithMany(p => p.Contracts)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Contracts__Custo__160F4887");

            entity.HasOne(d => d.PaymentMethod).WithMany(p => p.Contracts)
                .HasForeignKey(d => d.PaymentMethodId)
                .HasConstraintName("FK__Contracts__Payme__47DBAE45");

            entity.HasOne(d => d.Service).WithMany(p => p.Contracts)
                .HasForeignKey(d => d.ServiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Contracts__Servi__45F365D3");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("PK__Customer__A4AE64D8C0AAC022");

            entity.HasIndex(e => e.Phone, "UQ__Customer__5C7E359E524C8F89").IsUnique();

            entity.HasIndex(e => e.Phone, "UQ__Customer__5C7E359ED5D49250").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Customer__A9D10534518F1328").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Customer__A9D10534827D34AC").IsUnique();

            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CustomerName).HasMaxLength(255);
            entity.Property(e => e.CustomerType).HasMaxLength(255);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(15);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Hoạt động");

            entity.HasOne(d => d.Role).WithMany(p => p.Customers)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Customers__RoleI__17036CC0");
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.FeedbackId).HasName("PK__Feedback__6A4BEDD6BA1BFACE");

            entity.Property(e => e.FeedbackDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Chờ duyệt");

            entity.HasOne(d => d.Customer).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK__Feedbacks__Custo__17F790F9");

            entity.HasOne(d => d.Service).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.ServiceId)
                .HasConstraintName("FK__Feedbacks__Servi__18EBB532");
        });

        modelBuilder.Entity<InventoryItem>(entity =>
        {
            entity.HasKey(e => e.ItemId).HasName("PK__Inventor__727E838BCE65497B");

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ItemName).HasMaxLength(255);
            entity.Property(e => e.ItemType).HasMaxLength(50);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Có sẵn");
            entity.Property(e => e.Unit).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<News>(entity =>
        {
            entity.HasKey(e => e.NewsId).HasName("PK__News__954EBDF3C6517B9F");

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Image).HasMaxLength(255);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Hiển thị");
            entity.Property(e => e.Title).HasMaxLength(255);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__Orders__C3905BCF01574D6C");

            entity.Property(e => e.Discount)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.OrderDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PaymentDate).HasColumnType("datetime");
            entity.Property(e => e.PaymentStatus)
                .HasMaxLength(20)
                .HasDefaultValue("Chưa thanh toán");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Chờ xử lý");
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => e.OrderDetailId).HasName("PK__OrderDet__D3B9D36CFFDCEF55");

            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Đang chờ xác nhận");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK__OrderDeta__Order__19DFD96B");
        });

        modelBuilder.Entity<Partner>(entity =>
        {
            entity.HasKey(e => e.PartnerId).HasName("PK__Partners__39FD63121BCC30F1");

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PartnerName).HasMaxLength(255);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Đang hợp tác");
        });

        modelBuilder.Entity<PaymentMethod>(entity =>
        {
            entity.HasKey(e => e.PaymentMethodId).HasName("PK__PaymentM__DC31C1D3045EB59D");

            entity.HasIndex(e => e.MethodName, "UQ__PaymentM__218CFB1743867DA0").IsUnique();

            entity.Property(e => e.MethodName).HasMaxLength(255);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE1AD3958436");

            entity.HasIndex(e => e.RoleName, "UQ__Roles__8A2B61604D1AC7CA").IsUnique();

            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.ServiceId).HasName("PK__Services__C51BB00A185CA04C");

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ServiceName).HasMaxLength(255);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Đang hoạt động");
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.CategoryService).WithMany(p => p.Services)
                .HasForeignKey(d => d.CategoryServiceId)
                .HasConstraintName("FK_ServiceCategories");
        });

        modelBuilder.Entity<ServiceCategory>(entity =>
        {
            entity.HasKey(e => e.CategoryServiceId).HasName("PK__ServiceC__56154800A130EB16");

            entity.Property(e => e.CategoryServiceName).HasMaxLength(255);
        });

        modelBuilder.Entity<ServiceDescription>(entity =>
        {
            entity.HasKey(e => e.DescriptionId).HasName("PK__ServiceD__A58A9F8BC206AFCF");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.Service).WithMany(p => p.ServiceDescriptions)
                .HasForeignKey(d => d.ServiceId)
                .HasConstraintName("FK_ServiceDescriptions_Service");
        });

        modelBuilder.Entity<ServiceInventory>(entity =>
        {
            entity.HasKey(e => e.ServiceInventoryId).HasName("PK__ServiceI__0EF8880377F13251");

            entity.ToTable("ServiceInventory");

            entity.Property(e => e.QuantityRequired).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Item).WithMany(p => p.ServiceInventories)
                .HasForeignKey(d => d.ItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ServiceIn__ItemI__1BC821DD");

            entity.HasOne(d => d.Service).WithMany(p => p.ServiceInventories)
                .HasForeignKey(d => d.ServiceId)
                .HasConstraintName("FK_ServiceInventory_Service");
        });

        modelBuilder.Entity<ServicePrice>(entity =>
        {
            entity.HasKey(e => e.PriceId).HasName("PK__ServiceP__49575BAFC804F9A2");

            entity.Property(e => e.OfficeSize).HasMaxLength(50);
            entity.Property(e => e.Price).HasColumnType("decimal(15, 2)");
            entity.Property(e => e.ServiceType).HasMaxLength(50);
            entity.Property(e => e.TreeSize).HasMaxLength(50);

            entity.HasOne(d => d.Service).WithMany(p => p.ServicePrices)
                .HasForeignKey(d => d.ServiceId)
                .HasConstraintName("FK__ServicePr__Servi__412EB0B6");
        });

        modelBuilder.Entity<ServiceRequest>(entity =>
        {
            entity.HasKey(e => e.RequestId).HasName("PK__ServiceR__33A8519A40DECBD3");

            entity.ToTable(tb => tb.HasTrigger("trg_UpdateTotalAmount"));

            entity.Property(e => e.RequestId).HasColumnName("RequestID");
            entity.Property(e => e.RequestDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ScheduleId).HasColumnName("ScheduleID");
            entity.Property(e => e.ServiceId).HasColumnName("ServiceID");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Chờ xử lý");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Customer).WithMany(p => p.ServiceRequests)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK__ServiceRe__Custo__1DB06A4F");

            entity.HasOne(d => d.Price).WithMany(p => p.ServiceRequests)
                .HasForeignKey(d => d.PriceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ServiceRe__Price__6A30C649");

            entity.HasOne(d => d.Schedule).WithMany(p => p.ServiceRequests)
                .HasForeignKey(d => d.ScheduleId)
                .HasConstraintName("FK__ServiceRe__Sched__1EA48E88");

            entity.HasOne(d => d.Service).WithMany(p => p.ServiceRequests)
                .HasForeignKey(d => d.ServiceId)
                .HasConstraintName("FK__ServiceRe__Servi__1F98B2C1");
        });

        modelBuilder.Entity<Staff>(entity =>
        {
            entity.HasKey(e => e.StaffId).HasName("PK__Staffs__96D4AB170FBC4DD6");

            entity.HasIndex(e => e.Phone, "UQ__Staffs__5C7E359E45895632").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Staffs__A9D1053489FCDA46").IsUnique();

            entity.Property(e => e.Avatar).HasMaxLength(255);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.HireDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.Position).HasMaxLength(100);
            entity.Property(e => e.Salary).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.StaffName).HasMaxLength(255);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Đang làm");

            entity.HasOne(d => d.Role).WithMany(p => p.Staff)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK__Staffs__RoleId__2180FB33");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
