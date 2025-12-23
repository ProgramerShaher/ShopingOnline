using System.ComponentModel.DataAnnotations.Schema;

namespace ShopingOnline.Dto
{
    public class TeamDto
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string Jobs { get; set; }

        public string Email { get; set; }
        public string Password { get; set; }
      //  public string Username { get; set; }
        public string PhonNumber { get; set; }
        public string Image { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        [NotMapped]
        public IFormFile File { get; set; }
    }
}
