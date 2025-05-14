using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using ShopingOnline.Data;
using ShopingOnline.Models;

namespace ShopingOnline
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));



            var connectionString = builder.Configuration.GetConnectionString("ConnectionShopingOnline")
                ?? throw new InvalidOperationException("Connection string 'ConnectionShopingOnline' not found.");

            // تسجيل DbContext
            builder.Services.AddDbContext<DbShopingOnlineContext>(options =>
                options.UseSqlServer(connectionString));

            // تسجيل هوية المستخدم + الأدوار
            builder.Services.AddIdentity<User, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
            })
            .AddEntityFrameworkStores<DbShopingOnlineContext>()
            .AddDefaultTokenProviders();

            // إعداد مسار الـ Login في Area معينة
            builder.Services.ConfigureApplicationCookie(options =>
            {
                // تحديد المسار الخاص بـ Login في الـ Area المحددة
                options.LoginPath = "/Identity/Account/Login"; // تأكد من أن هذا المسار يتوافق مع الـ Area التي لديك
                options.AccessDeniedPath = "/Identity/Account/AccessDenied"; // إذا كنت تحتاج صفحة لرفض الوصول
            });

            // صفحة الأخطاء للمطور
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            // MVC + Razor Pages
            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages(); // ← مطلوب لـ MapRazorPages

            // Session
            builder.Services.AddDistributedMemoryCache(); // مطلوب لتخزين بيانات الجلسة في الذاكرة
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            builder.Services.AddHttpContextAccessor();


            builder.Services.AddTransient<IEmailSender, ShopingOnline.Services.FakeEmailSender>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();

            app.Run();
        }
    }
}
