using System.ComponentModel.DataAnnotations;

namespace ShopingOnline.Models
{
    public class Invoice
    {
        public int Id { get; set; }

        public int OrderId { get; set; }
        public Order Order { get; set; }

        [Required]
        public string InvoiceNumber { get; set; }

        public DateTime InvoiceDate { get; set; }

        public string PdfPath { get; set; }
    }

}
