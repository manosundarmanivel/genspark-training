namespace ChienVHShopOnline.Models
{
    public class Order
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

        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}
