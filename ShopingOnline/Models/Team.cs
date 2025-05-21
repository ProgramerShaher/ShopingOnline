using System.ComponentModel.DataAnnotations.Schema;

namespace ShopingOnline.Models
{
    public class Team
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string Jobs { get; set; }

        public string Email { get; set; }
        public string Image { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        [NotMapped]
        public IFormFile File { get; set; }
    }
}
