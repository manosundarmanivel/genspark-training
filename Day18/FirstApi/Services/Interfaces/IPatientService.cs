using FirstApi.Models;
using FirstApi.Models.DTOs;

namespace FirstApi.Services.Interfaces
{
    public interface IPatientService
    {
        Task<List<PatientDTO>> GetAllAsync();
        Task<PatientDTO?> GetByIdAsync(int id);
        Task AddAsync(PatientDTO dto);
        Task DeleteAsync(int id);
        Task<List<Appointment>> GetAppointmentsAsync(int patientId);
    }
}
