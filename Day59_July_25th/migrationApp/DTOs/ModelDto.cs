using System.ComponentModel.DataAnnotations;
namespace ChienVHShopOnline.Dtos
{
    public class ModelDto
    {
        public int ModelId { get; set; }
        public string Model1 { get; set; } = string.Empty;
    }

     public class CreateModelDto
    {
        [Required]
        public string Model1 { get; set; } = string.Empty;
    }

    public class UpdateModelDto
    {
        [Required]
        public int ModelId { get; set; }

        [Required]
        public string Model1 { get; set; } = string.Empty;
    }
}
