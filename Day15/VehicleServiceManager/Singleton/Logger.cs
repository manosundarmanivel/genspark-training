namespace VehicleServiceManager.Singleton;


//You only need one instance of a class throughout the entire application
//Creating multiple instances wastes memory or can cause inconsistency.
//Hided the constructor (private).
//Created a static property that stores the one-and-only instance.
//Provided thread-safe access to it.

public class Logger
{
    private static Logger? _instance;
    private static readonly object _lock = new();

    private Logger() { }

    public static Logger Instance
    {
        get
        {
            lock (_lock)
            {
                return _instance ??= new Logger();
            }
        }
    }

    public void Log(string message)
    {
        Console.WriteLine($"[LOG {DateTime.Now:HH:mm:ss}]: {message}");
    }
}
