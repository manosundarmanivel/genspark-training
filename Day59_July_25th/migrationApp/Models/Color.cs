namespace ChienVHShopOnline.Models
{
    public class Color
    {
        public int ColorId { get; set; }
        public string Color1 { get; set; }

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
