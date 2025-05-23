namespace ProxyDemo
{
public class ProxyFile : IFile
{
    private readonly File _realFile = new();
    private readonly User _user;

    public ProxyFile(User user)
    {
        _user = user;
    }

    public void Read()
    {
        switch (_user.Role.ToLower())
        {
            case "admin":
                _realFile.Read();
                break;
            case "user":
                Console.WriteLine("[Access Limited] Displaying file metadata only...");
                break;
            case "guest":
            default:
                Console.WriteLine("[Access Denied] You do not have permission to read this file.");
                break;
        }
    }
}

    public class ProxyDemoRunner
    {
        public static void Run()
        {
        Console.WriteLine("Enter your username:");
        string username = Console.ReadLine() ?? "Unknown";

        Console.WriteLine("Enter your role (Admin/User/Guest):");
        string role = Console.ReadLine() ?? "Guest";

        var user = new User(username, role);
        var file = new ProxyFile(user);

        Console.WriteLine($"\nUser: {user.Username} | Role: {user.Role}");
        file.Read();
        }
    }
}