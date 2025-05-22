using WholeApplication.Models;

namespace WholeApplication.Interfaces
{
    public interface INotificationService
    {
        void SendConfirmation(Appointment appointment);
    }
}