namespace VehicleServiceManager.Models;

public abstract class ServiceRecord
{
    public string OwnerName { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public abstract string VehicleType { get; }
    public abstract decimal GetServiceCost();
}
