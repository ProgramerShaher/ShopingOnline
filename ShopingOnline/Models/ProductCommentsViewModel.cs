using ShopingOnline.Models;
using System.ComponentModel.DataAnnotations;

public class ProductCommentsViewModel
{
    public Product Product { get; set; }
    public List<Comment> Comments { get; set; }

    [Required]
    [MaxLength(1000)]
    public string NewCommentText { get; set; }

    [Range(1, 5)]
    public int? NewStars { get; set; }

    public User User { get; set; }  // إضافة المستخدم ليكون مرتبطًا بالتعليق
}
