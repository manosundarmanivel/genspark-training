using VehicleServiceManager.Factory;
using VehicleServiceManager.Models;
using VehicleServiceManager.Singleton;
using VehicleServiceManager.AbstractFactory;

Logger.Instance.Log("Vehicle Service Manager Started.");

while (true)
{
    Console.WriteLine("\n--- Vehicle Service Manager ---");
    Console.Write("Enter Vehicle Type (Car/Bike) or 'exit': ");
    string? type = Console.ReadLine()?.ToLower();

    if (type == "exit") break;

    try
    {
        ServiceRecord record = ServiceRecordFactory.CreateServiceRecord(type);
        Console.Write("Enter Owner Name: ");
        record.OwnerName = Console.ReadLine() ?? "Unknown";
        record.Date = DateTime.Now;

        
        IServiceFactory factory = type switch
        {
            "car" => new CarServiceFactory(),
            "bike" => new BikeServiceFactory(),
            _ => throw new ArgumentException("Unknown type")
        };

        var price = factory.GetServicePrice();

        Logger.Instance.Log($"Created {record.VehicleType} service record for {record.OwnerName}.");
        Console.WriteLine($"\n--- Receipt ---\n" +
                          $"Owner: {record.OwnerName}\n" +
                          $"Vehicle: {record.VehicleType}\n" +
                          $"Date: {record.Date:yyyy-MM-dd}\n" +
                          $"Cost: ₹{price}");
    }
    catch (Exception ex)
    {
        Logger.Instance.Log("Error: " + ex.Message);
    }
}
