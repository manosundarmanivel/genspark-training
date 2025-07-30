using System.ComponentModel.DataAnnotations;
namespace ChienVHShopOnline.Dtos
{

public class NewsDto
{
    public int NewsId { get; set; }
    public int? UserId { get; set; }
    public string Title { get; set; }
    public string ShortDescription { get; set; }

    public string Image { get; set; } 

    public string Content { get; set; }
    public DateTime? CreatedDate { get; set; }
    public int? Status { get; set; }
}


public class CreateNewsDto
{
    public int? UserId { get; set; }

    [Required]
    public string Title { get; set; }

    public string ShortDescription { get; set; }

    public IFormFile? ImageFile { get; set; }

    public string? Image { get; set; } 

    public string Content { get; set; }
    public DateTime? CreatedDate { get; set; } = DateTime.Now;
    public int? Status { get; set; } = 1;
}
}
