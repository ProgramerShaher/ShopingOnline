using System.ComponentModel.DataAnnotations;

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

        public int CategoryId { get; set; }
        public Category Category { get; set; }

        // علاقات
        public ICollection<OrderDetail> OrderDetails { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<CartItem> CartItems { get; set; }
    }

}
