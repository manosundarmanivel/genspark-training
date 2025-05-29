using SecondWebApi.Interfaces.Mappers;
using SecondWebApi.Models;
using SecondWebApi.Models.Dtos;

namespace SecondWebApi.Services.Mappers
{
    public class DoctorMapper : IDoctorMapper
    {
        public Doctor MapFromAddDto(DoctorAddDto dto)
        {
            return new Doctor
            {
                Name = dto.Name,
                YearsOfExperience = dto.YearsOfExperience,
                Status = "Created"
            };
        }


        public DoctorSpecialityResponseDto MapToSpecialityResponse(Doctor doctor)
        {
            return new DoctorSpecialityResponseDto
            {
                Id = doctor.Id,
                Name = doctor.Name,
                YearsOfExperience = doctor.YearsOfExperience,
                
            };
        }
    }
}
