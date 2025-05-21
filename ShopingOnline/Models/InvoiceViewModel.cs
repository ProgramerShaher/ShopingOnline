namespace ShopingOnline.Models
{
    public class InvoiceViewModel
    {
        public int OrderId { get; set; }
        public string CustomerName { get; set; }
        public string Email { get; set; }
        public DateTime OrderDate { get; set; }
        public List<InvoiceProductItem>
        Items
        { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; }
    }

    public class InvoiceProductItem
    {
        public string ProductName { get; set; }
        public string Category { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

}
