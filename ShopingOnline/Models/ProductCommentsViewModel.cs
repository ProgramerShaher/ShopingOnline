using ShopingOnline.Models;
using System.ComponentModel.DataAnnotations;

public class ProductCommentsViewModel
{
    public Product Product { get; set; }
    public List<Comment> Comments { get; set; }

    public int ProductId { get; set; }  // ⬅️ مهم!

    [Required]
    [MaxLength(1000)]
    public string NewCommentText { get; set; }

    [Range(1, 5)]
    public int? NewStars { get; set; }

    public User User { get; set; }
}
