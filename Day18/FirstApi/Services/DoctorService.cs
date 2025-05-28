using FirstApi.Data;
using FirstApi.Models;
using FirstApi.Models.DTOs;
using FirstApi.Repositories.Interfaces;
using FirstApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FirstApi.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly IRepository<Doctor> _repo;

private readonly AppDbContext _context;

public DoctorService(IRepository<Doctor> repo, AppDbContext context)
{
    _repo = repo;
    _context = context;
}

        public async Task<List<DoctorDTO>> GetAllAsync()
        {
            var doctors = await _repo.GetAllAsync();
            return doctors.Select(MapToDTO).ToList();
        }

        public async Task<DoctorDTO?> GetByIdAsync(int id)
        {
            var doctor = await _repo.GetByIdAsync(id);
            return doctor is null ? null : MapToDTO(doctor);
        }

       public async Task AddAsync(DoctorDTO dto)
{
    var doctor = new Doctor
    {
        Name = dto.Name,
        Specialization = dto.Specialization,
        PhoneNumber = dto.PhoneNumber,
        Email = dto.Email,
        DoctorSpecialities = new List<DoctorSpeciality>()
    };

    if (dto.Specialities != null)
    {
        foreach (var specialityName in dto.Specialities)
        {
            var speciality = await _context.Specialities
                .FirstOrDefaultAsync(s => s.Name.ToLower() == specialityName.ToLower());

            if (speciality == null)
            {
                speciality = new Speciality { Name = specialityName };
                _context.Specialities.Add(speciality);
                await _context.SaveChangesAsync();
            }

            doctor.DoctorSpecialities.Add(new DoctorSpeciality
            {
                Doctor = doctor,
                Speciality = speciality
            });
        }
    }

    await _repo.AddAsync(doctor);
}


        public async Task DeleteAsync(int id)
        {
            await _repo.DeleteAsync(id);
        }

        public async Task<List<DoctorDTO>> GetBySpecialityAsync(string specialityName)
        {
            var doctors = await _repo.GetAllAsync();
            var filtered = doctors
                .Where(d => d.DoctorSpecialities != null &&
                            d.DoctorSpecialities.Any(ds =>
                                ds.Speciality != null &&
                                ds.Speciality.Name.Equals(specialityName, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            return filtered.Select(MapToDTO).ToList();
        }

        private DoctorDTO MapToDTO(Doctor doctor)
        {
            return new DoctorDTO
            {
                Id = doctor.Id,
                Name = doctor.Name,
                Specialization = doctor.Specialization,
                PhoneNumber = doctor.PhoneNumber,
                Email = doctor.Email,
                Specialities = doctor.DoctorSpecialities?
                    .Select(ds => ds.Speciality?.Name ?? "")
                    .Where(name => !string.IsNullOrWhiteSpace(name))
                    .ToList()
            };
        }

        private Doctor MapToEntity(DoctorDTO dto)
        {
            return new Doctor
            {
                Id = dto.Id,
                Name = dto.Name,
                Specialization = dto.Specialization,
                PhoneNumber = dto.PhoneNumber,
                Email = dto.Email,
                DoctorSpecialities = new List<DoctorSpeciality>()
            };
        }
    }
}
