using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using ShopingOnline.Models;

namespace ShopingOnline.Data
{
    // استخدم IdentityDbContext مع IdentityUser أو ApplicationUser إذا كنت تريد تخصيصه
    public class DbShopingOnlineContext : IdentityDbContext<User>
    {
        public DbShopingOnlineContext(DbContextOptions<DbShopingOnlineContext> options)
            : base(options)
        {
        }

        // الحفاظ على جميع الـ DbSets الحالية
        public DbSet<User> Users { get; set; } // سيتم استبدالها بجدول AspNetUsers
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Team> Teammembers { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            // أو استخدم اسم الجدول القديم بشكل صريح
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Order>()
           .Property(o => o.OrderStatusNew)
           .HasConversion<string>();

        }

    }
}