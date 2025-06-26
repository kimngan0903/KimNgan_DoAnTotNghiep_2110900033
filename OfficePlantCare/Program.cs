using Microsoft.EntityFrameworkCore;
using OfficePlantCare.Models;
using OfficePlantCare.Areas.AdminQL.Models;
using OfficePlantCare.Areas.AdminQL.Controllers;

namespace OfficePlantCare
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<DashboardService>();
            builder.Services.AddDbContext<OfficePlantCareContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("OfficePlantCareConnection")));

            // Cấu hình Session
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(6); // Thời gian hết hạn session
                options.Cookie.HttpOnly = true; // Bảo vệ cookie khỏi truy cập JavaScript
                options.Cookie.IsEssential = true; // Đảm bảo cookie được gửi ngay cả khi không có đồng ý GDPR
                options.Cookie.Name = "OfficePlantCare"; // Tên cookie session
                options.Cookie.SameSite = SameSiteMode.Lax; // Cho phép cookie trong các yêu cầu chuyển hướng
                options.Cookie.SecurePolicy = builder.Environment.IsDevelopment() ? CookieSecurePolicy.None : CookieSecurePolicy.Always; // Tùy chỉnh cho development
            });

            // Đăng ký CareScheduleService với scoped lifetime
            builder.Services.AddScoped<CareSchedulesController>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            // Sử dụng session trước UseRouting
            app.UseSession();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "qrcode",
                pattern: "QrCode/{action=Index}/{id?}");

            app.Run();
        }
    }
}