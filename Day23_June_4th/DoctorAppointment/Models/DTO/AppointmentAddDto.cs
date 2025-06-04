namespace DoctorAppointment.Models.DTO
{
    public class AppointmentAddDto
    {
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public DateTime AppointmnetDateTime { get; set; }
    }
}
