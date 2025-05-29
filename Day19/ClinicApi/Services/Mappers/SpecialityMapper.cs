using SecondWebApi.Interfaces.Mappers;
using SecondWebApi.Models;
using SecondWebApi.Models.Dtos;

namespace SecondWebApi.Services.Mappers
{
    public class SpecialityMapper : ISpecialityMapper
    {
        public Speciality MapFromDto(SpecialityAddDto dto)
        {
            return new Speciality
            {
                Name = dto.Name,
                Status = "Created"
            };
        }

        
    }
}
