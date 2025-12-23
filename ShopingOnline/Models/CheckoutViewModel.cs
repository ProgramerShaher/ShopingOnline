using ShopingOnline.Models;

namespace ShopingOnline.ViewModels
{
    public class CheckoutViewModel
    {
        // من جدول Users
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }

        // بيانات السلة
        public List<CartItem> CartItems { get; set; }

        // للحساب
        public decimal SubTotal { get; set; }
        public decimal Shipping { get; set; } // مثال شحن ثابت
        public decimal Total => SubTotal + Shipping;
    }
}
