namespace ChienVHShopOnline.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public ICollection<News> News { get; set; } = new List<News>();
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
