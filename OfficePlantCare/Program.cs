﻿using Microsoft.EntityFrameworkCore;
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
            // C?u hình s? d?ng session
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(6);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.Cookie.Name = "OfficePlantCare";  // Đặt tên cookie cho session
            });
            // Đăng ký CareScheduleService với scoped lifetime
            builder.Services.AddScoped<CareScheduleController>();
            var app = builder.Build();
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseSession();
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

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
