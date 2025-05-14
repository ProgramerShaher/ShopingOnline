using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopingOnline.Models
{
    public class Order
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        [ForeignKey("UserId")]

        public User User { get; set; }

        public DateTime OrderDate { get; set; }

        [Required]
        public string Status { get; set; } = "قيد المعالجة";

        public decimal TotalAmount { get; set; }

        // علاقات
        public ICollection<OrderDetail> OrderDetails { get; set; }
        public Invoice Invoice { get; set; }
    }

}
