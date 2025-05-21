namespace ShopingOnline.Models
{
    public class OrderViewWithProduct
    {
        public int OrderId { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? Description { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatus Status { get; set; }
        public decimal TotalAmount { get; set; }
        public string image { get; set; }
        // المنتجات المرتبطة بالطلب
        public List<string> ProductNames { get; set; } = new();
        public List<string> ProductDescriptions { get; set; } = new();
        public string product { get; set; }
        // الأصناف المرتبطة بالطلب
        public List<string> ProductCategories { get; set; } = new();
    }


}
