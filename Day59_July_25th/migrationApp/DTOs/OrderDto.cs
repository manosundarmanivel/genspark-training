using System.ComponentModel.DataAnnotations;
namespace ChienVHShopOnline.Dtos
{
    public class OrderDto
    {
        public int OrderID { get; set; }
        public string OrderName { get; set; }
        public DateTime? OrderDate { get; set; }
        public string PaymentType { get; set; }
        public string Status { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerAddress { get; set; }
    }

    public class CreateOrderDto
    {
        [Required]
        public string OrderName { get; set; }
        public DateTime? OrderDate { get; set; }
        public string PaymentType { get; set; }
        public string Status { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerAddress { get; set; }
    }
    
    public class UpdateOrderDto
    {
        [Required]
        public int OrderID { get; set; }

        [Required]
        public string OrderName { get; set; }

        public DateTime? OrderDate { get; set; }
        public string PaymentType { get; set; }
        public string Status { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerAddress { get; set; }
    }
}
