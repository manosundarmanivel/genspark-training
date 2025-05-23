namespace ProxyDemo
{
    public class User
    {
        public string Username { get; set; }
        public string Role { get; set; } 

        public User(string username, string role)
        {
            Username = username;
            Role = role;
        }
    }
}