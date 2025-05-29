
using SecondWebApi.Models;
using SecondWebApi.Models.Dtos;

namespace SecondWebApi.Interfaces;

public interface ISpecialityService
{
    public Task<Speciality?> AddSpeciality(SpecialityAddDto specialityAddDto);
    public Task<IEnumerable<Speciality>?> GetSpecialities();
}