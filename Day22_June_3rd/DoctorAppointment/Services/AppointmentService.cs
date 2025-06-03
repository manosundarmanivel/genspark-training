
using DoctorAppointment.Interfaces;
using DoctorAppointment.Models;
using DoctorAppointment.Models.DTO;
using DoctorAppointment.Models.DTOs.DoctorSpecialities;
using DoctorAppointment.Interfaces;

namespace FirstAPI.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly AppointmentRepo _appointmentRepo;
        private readonly IRepository<int, Doctor> _doctorRepo;
        private readonly IRepository<int, Patient> _patientRepo;

       public AppointmentService(AppointmentRepo appointmentRepo, IRepository<int, Doctor> doctorRepo, IRepository<int, Patient> patientRepo)
{
    _appointmentRepo = appointmentRepo;
    _doctorRepo = doctorRepo;
    _patientRepo = patientRepo;
}

        public async Task<Appointment> AddAppointment(AppointmentAddDto appointmentDto)
        {
            var doctor = await _doctorRepo.Get(appointmentDto.DoctorId)
                         ?? throw new Exception("Doctor not found");

            var patient = await _patientRepo.Get(appointmentDto.PatientId)
                          ?? throw new Exception("Patient not found");

            var appointment = new Appointment
            {
                DoctorId = appointmentDto.DoctorId,
                PatientId = appointmentDto.PatientId,
                AppointmnetDateTime = appointmentDto.AppointmnetDateTime,
                Status = "Scheduled"
            };

            return await _appointmentRepo.Add(appointment);
        }

        public async Task<Appointment> GetAppointment(int appointmentId)
        {
            var appointment = await _appointmentRepo.Get(appointmentId);
            if (appointment == null)
                throw new Exception("Appointment not found");

            return appointment;
        }




        public async Task<bool> CancelAppointment(int appointmentId, int doctorId)
        {
            var doctor = await _doctorRepo.Get(doctorId);
            if (doctor == null)
                throw new Exception("Doctor not found");

            var appointment = await _appointmentRepo.Get(appointmentId);
            if (appointment == null)
                throw new Exception("Appointment not found");

            if (appointment.DoctorId != doctorId)
                throw new UnauthorizedAccessException("You can only cancel your own appointments.");

            appointment.Status = "Cancelled";
            await _appointmentRepo.Update(appointment);
            return true;
        }
    }

}