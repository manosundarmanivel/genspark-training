using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DoctorAppointment.Attributes;
namespace DoctorAppointment.Models
{
    public class Patient
    {
       [Key]
        public int Id { get; set; }
        [NameValidation]
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public ICollection<Appointment>? Appointments { get; set; }
         public User? User { get; set; }
    }

}