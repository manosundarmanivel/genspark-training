namespace SecondWebApi.Models.Dtos;

public class AppointmentAddDto
{
    public int PatientId { get; set; }
    public int DoctorId { get; set; }
    public DateTime AppointmnetDateTime { get; set; }
}