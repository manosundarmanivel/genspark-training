using WholeApplication.Models;
using WholeApplication.Interfaces;

namespace WholeApplication.Services
{
    public class AppointmentService
    {
        private readonly IAppointmentRepository _repository;
        private readonly INotificationService _notifier;

        public AppointmentService(IAppointmentRepository repository, INotificationService notifier)
        {
            _repository = repository;
            _notifier = notifier;
        }

        public void BookAppointment(Appointment appointment)
        {
            _repository.Save(appointment);
            _notifier.SendConfirmation(appointment);
        }

        public void ViewAppointments(DateTime date)
        {
            var appointments = _repository.GetAppointments(date);
            if (appointments.Count == 0)
            {
                Console.WriteLine("No appointments found.");
                return;
            }

            foreach (var a in appointments)
            {
                Console.WriteLine($"{a.PatientName} - {a.Date}");
            }
        }

        public void CancelAppointment(string patientName, DateTime date)
        {
            var appointment = _repository.GetAppointments(date)
                .FirstOrDefault(a => a.PatientName.Equals(patientName, StringComparison.OrdinalIgnoreCase));

            if (appointment != null)
            {
                _repository.Remove(appointment);
                Console.WriteLine("Appointment cancelled.");
            }
            else
            {
                Console.WriteLine("Appointment not found.");
            }
        }

        public void RescheduleAppointment(string patientName, DateTime oldDate, DateTime newDate)
        {
            var appointment = _repository.GetAppointments(oldDate)
                .FirstOrDefault(a => a.PatientName.Equals(patientName, StringComparison.OrdinalIgnoreCase));

            if (appointment != null)
            {
                _repository.Remove(appointment);
                appointment.Date = newDate;
                _repository.Save(appointment);
                Console.WriteLine("Appointment rescheduled.");
            }
            else
            {
                Console.WriteLine("Appointment not found.");
            }
        }
    }
}