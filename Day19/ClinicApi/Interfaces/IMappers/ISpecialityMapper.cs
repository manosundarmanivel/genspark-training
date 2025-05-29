using SecondWebApi.Models;
using SecondWebApi.Models.Dtos;

namespace SecondWebApi.Interfaces.Mappers
{
    public interface ISpecialityMapper
    {
        Speciality MapFromDto(SpecialityAddDto dto);
    }
}
