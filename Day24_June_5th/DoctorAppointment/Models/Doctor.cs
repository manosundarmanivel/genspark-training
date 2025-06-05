using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DoctorAppointment.Attributes;
namespace DoctorAppointment.Models
{


    public class Doctor
    {   
        [Key]
        public int Id { get; set; }

        [NameValidation]
        public string Name { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
         public string Email { get; set; } = string.Empty;
        public float YearsOfExperience { get; set; }
        public ICollection<DoctorSpeciality>? DoctorSpecialities { get; set; }
        public ICollection<Appointment>? Appointmnets { get; set; }
         public User? User { get; set; }

    }
}