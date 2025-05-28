using FirstApi.Models;
using FirstApi.Services.Interfaces;
using FirstApi.Models.DTOs;
public interface IDoctorService 
{
    Task<List<DoctorDTO>> GetAllAsync();
        Task<DoctorDTO?> GetByIdAsync(int id);
        Task AddAsync(DoctorDTO dto);
        Task DeleteAsync(int id);
    Task<List<DoctorDTO>> GetBySpecialityAsync(string specialityName);
}
