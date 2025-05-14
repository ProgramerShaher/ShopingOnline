using System.ComponentModel.DataAnnotations.Schema;

namespace ShopingOnline.Models
{
    public class CartItem
    {
        public int Id { get; set; }

        public string UserId { get; set; }  // 👈 لأنك تستخدم Identity

        [ForeignKey("UserId")]
        public User User { get; set; }  // 👈 هذا هو الكلاس المخصص للمستخدمين

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int Quantity { get; set; }
    }


}
