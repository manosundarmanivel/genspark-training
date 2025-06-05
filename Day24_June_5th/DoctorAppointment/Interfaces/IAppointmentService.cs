
using DoctorAppointment.Models.DTO;
using DoctorAppointment.Models.DTOs.DoctorSpecialities;
using DoctorAppointment.Models;


namespace DoctorAppointment.Interfaces
{
    public interface IAppointmentService
    {
        Task<Appointment> AddAppointment(AppointmentAddDto appointmentDto);
        Task<bool> CancelAppointment(int appointmentId, int doctorId);

       Task<Appointment> GetAppointment(int appointmentId);
    }

}