using VehicleServiceManager.Models;

namespace VehicleServiceManager.Factory;

//Factory Pattern
// if you're creating different types of objects (Car, Bike, Truck). 

// if (type == "car") return new Car();
// else if (type == "bike") return new Bike();

//create a factory class with a method that returns an object.
//This method decides which class to instantiate based on input.
public static class ServiceRecordFactory
{
    public static ServiceRecord CreateServiceRecord(string type)
    {
        return type.ToLower() switch
        {
            "car" => new CarServiceRecord(),
            "bike" => new BikeServiceRecord(),
            _ => throw new ArgumentException("Unsupported vehicle type.")
        };
    }
}

//When object creation varies, but the interface stays the same.
