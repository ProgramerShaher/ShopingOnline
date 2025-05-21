namespace ShopingOnline.Models
{
    public class OrderDetailsViewModel
    {
        public int OrderId { get; set; }
        public string UserName { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatus Status { get; set; }
        public List<OrderProductViewModel> Products { get; set; } = new();
    }

    public class OrderProductViewModel
    {
        public string ProductName { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public decimal Price { get; set; }
    }
}
