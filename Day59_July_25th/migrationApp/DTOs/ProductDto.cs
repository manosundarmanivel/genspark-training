using System.ComponentModel.DataAnnotations;
namespace ChienVHShopOnline.Dtos
{
    public class ProductDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Image { get; set; }
        public double? Price { get; set; }
        public int? UserId { get; set; }
        public int? CategoryId { get; set; }
        public int? ColorId { get; set; }
        public int? ModelId { get; set; }
        public DateTime? SellStartDate { get; set; }
        public DateTime? SellEndDate { get; set; }
        public int? IsNew { get; set; }
    }

    public class CreateProductDto
    {
        [Required]
        public string ProductName { get; set; }

        public string Image { get; set; }
        public double? Price { get; set; }

        public int? UserId { get; set; }
        public int? CategoryId { get; set; }
        public int? ColorId { get; set; }
        public int? ModelId { get; set; }

        public DateTime? SellStartDate { get; set; }
        public DateTime? SellEndDate { get; set; }
        public int? IsNew { get; set; }
    }

    public class UpdateProductDto
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        public string ProductName { get; set; }

        public string Image { get; set; }
        public double? Price { get; set; }

        public int? UserId { get; set; }
        public int? CategoryId { get; set; }
        public int? ColorId { get; set; }
        public int? ModelId { get; set; }

        public DateTime? SellStartDate { get; set; }
        public DateTime? SellEndDate { get; set; }
        public int? IsNew { get; set; }
    }
    
}
