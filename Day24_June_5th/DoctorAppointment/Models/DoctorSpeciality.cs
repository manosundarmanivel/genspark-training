using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
namespace DoctorAppointment.Models
{
    public class DoctorSpeciality
    {   
        public int SerialNumber { get; set; }
        public int DoctorId { get; set; }
        public int SpecialityId { get; set; }
       [JsonIgnore]
        public Speciality? Speciality { get; set; }
        public Doctor? Doctor { get; set; }
    }
}