using SecondWebApi.Models;
using SecondWebApi.Models.Dtos;

namespace SecondWebApi.Interfaces.Mappers
{
    public interface IDoctorMapper
    {
        Doctor MapFromAddDto(DoctorAddDto dto);

        DoctorSpecialityResponseDto MapToSpecialityResponse(Doctor doctor);
    }
}
