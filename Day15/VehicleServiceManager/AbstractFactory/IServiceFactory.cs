using VehicleServiceManager.Models;

namespace VehicleServiceManager.AbstractFactory;


// Factory Pattern works good for single objects.

// Create a family of related objects?
// For example, a CarServiceFactory that produces:
// A CarServiceRecord and 
// A CarServicePricing

public interface IServiceFactory
{
    ServiceRecord CreateServiceRecord();
    decimal GetServicePrice();
}
