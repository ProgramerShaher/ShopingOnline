using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopingOnline.Models
{
    [Table("Users")] // اسم الجدول الموجود في قاعدة البيانات
    public class User : IdentityUser
    {
        [Required, MaxLength(100)]
        public string FullName { get; set; }

        [Column("Create")]
        public DateTime? Create { get; set; }

        [Column("Role")]
        public string? Role { get; set; }

        // علاقات (افترضت أنها اختيارية)
        public ICollection<Order>? Orders { get; set; }
        public ICollection<Comment>? Comments { get; set; }
    }
}
