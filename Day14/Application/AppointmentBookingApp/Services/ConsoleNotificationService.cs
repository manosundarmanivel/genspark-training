using WholeApplication.Interfaces;
using WholeApplication.Models;

namespace WholeApplication.Services
{
    public class ConsoleNotificationService : INotificationService
    {
        public void SendConfirmation(Appointment appointment)
        {
            Console.WriteLine($"Appointment confirmed for {appointment.PatientName} at {appointment.Date}");
        }
    }
}