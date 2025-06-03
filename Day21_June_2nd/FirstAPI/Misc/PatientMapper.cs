using FirstAPI.Models;
using FirstAPI.Models.DTOs.PatientAddRequestDto;

namespace FirstAPI.Misc
{
    public class PatientMapper
    {
        public Patient MapPatientAddRequestToPatient(PatientAddRequestDto addRequestDto)
        {
            return new Patient
            {
                Name = addRequestDto.Name,
                Age = addRequestDto.Age,
                Email = addRequestDto.Email,
                Phone = addRequestDto.Phone
            };
        }
    }
}
