using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopingOnline.Models
{
    public class Comment
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }
        [ForeignKey("UserId")]

        public User User { get; set; }

        [Required]
        public int ProductId { get; set; }
        public Product Product { get; set; }

        [Required, MaxLength(1000)]
        public string CommentText { get; set; }

        [Range(1, 5)]
        public int? Stars { get; set; }  // ممكن يكون تقييم نجوم من 1 إلى 5

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }

}
