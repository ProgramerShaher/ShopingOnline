using System.ComponentModel.DataAnnotations;

namespace ShopingOnline.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(255)]
        public string Description { get; set; }
        public string Image { get; set; }

        // علاقات
        public ICollection<Product> Products { get; set; }
    }

}
