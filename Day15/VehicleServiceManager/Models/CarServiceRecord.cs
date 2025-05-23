namespace VehicleServiceManager.Models;

public class CarServiceRecord : ServiceRecord
{
    public override string VehicleType => "Car";
    public override decimal GetServiceCost() => 1500.0m;
}
