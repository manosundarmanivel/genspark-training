namespace AdapterDemo
{
    public class TemperatureAdapter : ICelsiusSensor
    {
        private readonly FahrenheitSensor _sensor;

        public TemperatureAdapter(FahrenheitSensor sensor)
        {
            _sensor = sensor;
        }

        public double GetTemperatureCelsius()
        {
            return (_sensor.GetTempFahrenheit() - 32) * 5 / 9;
        }
    }

    public class AdapterDemoRunner
    {
        public static void Run()
        {
            ICelsiusSensor sensor = new TemperatureAdapter(new FahrenheitSensor());
            Console.WriteLine($"Temperature in Celsius: {sensor.GetTemperatureCelsius():F2}°C");
        }
    }
}