namespace SecondWebApi.Models;
public class Speciality
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public string Status { get; set; } = "Active";

    public ICollection<DoctorSpeciality>? DoctorSpecialities { get; set; }
}