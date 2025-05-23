using VehicleServiceManager.Models;

namespace VehicleServiceManager.AbstractFactory;

public class BikeServiceFactory : IServiceFactory
{
    public ServiceRecord CreateServiceRecord() => new BikeServiceRecord();
    public decimal GetServicePrice() => 500.0m;
}
