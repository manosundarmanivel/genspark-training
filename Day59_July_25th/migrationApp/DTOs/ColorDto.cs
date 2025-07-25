using System.ComponentModel.DataAnnotations;
namespace ChienVHShopOnline.Dtos
{
    public class ColorDto
    {
        public int ColorId { get; set; }
        public string ColorName { get; set; } = string.Empty;
    }

    public class CreateColorDto
    {
        [Required]
        [StringLength(50)]
        public string ColorName { get; set; } = string.Empty;
    }

    public class UpdateColorDto
    {
        public int ColorId { get; set; }

        [Required]
        [StringLength(50)]
        public string ColorName { get; set; } = string.Empty;
    }
    
}
