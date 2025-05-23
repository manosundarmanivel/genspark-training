namespace VehicleServiceManager.Models;

public class BikeServiceRecord : ServiceRecord
{
    public override string VehicleType => "Bike";
    public override decimal GetServiceCost() => 500.0m;
}
