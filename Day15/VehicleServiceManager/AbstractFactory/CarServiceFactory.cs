using VehicleServiceManager.Models;

namespace VehicleServiceManager.AbstractFactory;

public class CarServiceFactory : IServiceFactory
{
    public ServiceRecord CreateServiceRecord() => new CarServiceRecord();
    public decimal GetServicePrice() => 1500.0m;
}
