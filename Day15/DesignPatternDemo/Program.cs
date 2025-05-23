using ProxyDemo;
using AdapterDemo;
using FlyweightDemo;

class Program
{
    static void Main()
    {
        Console.WriteLine("=== Design Pattern Demo Center ===");
        Console.WriteLine("1. Proxy Pattern");
        Console.WriteLine("2. Adapter Pattern");
        Console.WriteLine("3. Flyweight Pattern");
        Console.Write("Choose a demo (1-3): ");

        string choice = Console.ReadLine() ?? "";

        switch (choice)
        {
            case "1":
                ProxyDemoRunner.Run();
                break;
            case "2":
                AdapterDemoRunner.Run();
                break;
            case "3":
                FlyweightDemoRunner.Run();
                break;
            default:
                Console.WriteLine("Invalid choice.");
                break;
        }
    }
}