

using System.Threading.Tasks;
using SecondWebApi.Interfaces;
using SecondWebApi.Interfaces.Mappers;
using SecondWebApi.Models;
using SecondWebApi.Models.Dtos;

public class SpecialityService : ISpecialityService
{
    private readonly IRepository<int, Speciality> _repository;
    private readonly ISpecialityMapper _specialityMapper;
    public SpecialityService(IRepository<int, Speciality> repository, ISpecialityMapper specialityMapper)
    {
        _repository = repository;
        _specialityMapper = specialityMapper;
    }

    public async Task<Speciality?> AddSpeciality(SpecialityAddDto specialityAddDto)
    {
        try
        {
            var speciality = _specialityMapper.MapFromDto(specialityAddDto);
            var spec = await _repository.Add(new Speciality { Name = specialityAddDto.Name });
            return spec;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error Adding Speciality :{ex.Message}");
            return null;
        }
    }

    public async Task<IEnumerable<Speciality>?> GetSpecialities()
    {
        try
        {
            var specialities = await _repository.GetAll();
            return specialities;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error Adding Speciality :{ex.Message}");
             return new List<Speciality>();
        }
    }

}