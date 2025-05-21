using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopingOnline.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public string Image { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        [NotMapped]
        public IFormFile File { get; set; }
        // علاقات
        public ICollection<OrderDetail> OrderDetails { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<CartItem> CartItems { get; set; }
    }

}
