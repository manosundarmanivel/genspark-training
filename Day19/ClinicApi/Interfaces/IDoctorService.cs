
using SecondWebApi.Models;
using SecondWebApi.Models.Dtos;

namespace SecondWebApi.Interfaces;

public interface IDoctorService
{
    public Task<Doctor> GetDoctorByName(string name);
    public Task<ICollection<DoctorSpecialityResponseDto>> GetDoctorsBySpeciality(string speciality);
    public Task<Doctor> AddDoctor(DoctorAddDto doctorAddDto);
}