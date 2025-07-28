using System.ComponentModel.DataAnnotations;
namespace ChienVHShopOnline.Dtos
{
    public class OrderDetailDto
    {
        public int OrderID { get; set; }
        public int ProductID { get; set; }
        public double? Price { get; set; }
        public int? Quantity { get; set; }
    }

    public class CreateOrderDetailDto
    {
        [Required]
        public int OrderID { get; set; }

        [Required]
        public int ProductID { get; set; }

        public double? Price { get; set; }
        public int? Quantity { get; set; }
    }

    
}
