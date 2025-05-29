using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
namespace SecondWebApi.Models;
public class DoctorSpeciality
{
   
    public int SerialNumber { get; set; }
    public int DoctorId { get; set; }
    public int SpecialityId { get; set; }
  
    public Speciality? Speciality { get; set; }
    [JsonIgnore] 
    public Doctor? Doctor { get; set; }
}