using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FirstApi.Models
{
    public class DoctorSpeciality
    {
    
    public int DoctorId { get; set; }
    public int SpecialityId { get; set; }
  
    public Doctor Doctor { get; set; }

    public Speciality Speciality { get; set; }
    }
}