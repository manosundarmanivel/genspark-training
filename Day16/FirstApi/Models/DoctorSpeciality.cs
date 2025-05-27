using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FirstApi.Models
{
    public class DoctorSpeciality
    {
    [Key]
    public int DoctorId { get; set; }
    public int SpecialityId { get; set; }
    [ForeignKey("DoctorId")]
    public Doctor Doctor { get; set; }
    [ForeignKey("SpecialityId")]
    public Speciality Speciality { get; set; }
    }
}