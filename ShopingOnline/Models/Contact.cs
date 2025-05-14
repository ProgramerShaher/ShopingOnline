using System.ComponentModel.DataAnnotations;

namespace ShopingOnline.Models
{
    public class Contact
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Subject { get; set; }

        [Required]
        public string Message { get; set; }

        public DateTime SentDate { get; set; } = DateTime.Now;
    }

}
