namespace ChienVHShopOnline.Models
{
    public class Model
    {
        public int ModelId { get; set; }
        public string Model1 { get; set; }

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
