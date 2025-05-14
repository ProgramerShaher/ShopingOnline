using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace ShopingOnline.Models
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

       
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

           

            // أو استخدم اسم الجدول القديم بشكل صريح
            modelBuilder.Entity<User>().ToTable("Users");
        }
    }
}