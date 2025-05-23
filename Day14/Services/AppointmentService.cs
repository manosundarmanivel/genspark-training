
using Application.Interfaces;
using Application.Models;
using Application.Repositories;


namespace Application.Services
{
    public class AppointmentService
    {
        private readonly IAppointmentRepository _repository;
        private readonly INotificationService _notifier;

        public AppointmentService(IAppointmentRepository repo, INotificationService notifier)
        {
            _repository = repo;
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
            foreach (var appt in appointments)
            {
                Console.WriteLine($"{appt.PatientName} - {appt.Date}");
            }
        }
    }

}
