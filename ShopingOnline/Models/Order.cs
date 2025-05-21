using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopingOnline.Models
{
    public enum OrderStatus
    {
        جديد,          // New (initial status)
        قيد_المعالجة,  // Being processed
        مكتمل,         // Delivered or Completed
        ملغي           // Cancelled
    }

    public class Order
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        [ForeignKey("UserId")]

        public User User { get; set; }

        public DateTime OrderDate { get; set; }

        [Required]
        public OrderStatus OrderStatusNew { get; set; } = OrderStatus.جديد;

        public decimal TotalAmount { get; set; }

        // علاقات
        public ICollection<OrderDetail> OrderDetails { get; set; }
        public Invoice Invoice { get; set; }
    }

}
