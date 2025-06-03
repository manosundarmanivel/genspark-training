using FirstAPI.Models;
using FirstAPI.Models.DTOs.PatientAddRequestDto;

namespace FirstAPI.Misc
{
    public class PatientMapper
    {
        public Patient? MapPatientAddRequestDoctor(PatientAddRequestDto addRequestDto)
        {
            Patient patient = new();
            patient.Name = addRequestDto.Name;
            patient.
            doctor. = addRequestDto.YearsOfExperience;
            doctor.Email = addRequestDto.Email;
            return doctor;
        }
    }
}