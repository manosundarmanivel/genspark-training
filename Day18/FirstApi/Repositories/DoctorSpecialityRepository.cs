using FirstApi.Data;
using FirstApi.Models;
using FirstApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FirstApi.Repositories
{
    public class DoctorSpecialityRepository : IRepository<DoctorSpeciality>
    {
        private readonly AppDbContext _context;
         public DoctorSpecialityRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task AddAsync(DoctorSpeciality entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<DoctorSpeciality>> GetAllAsync()
        {
            return await _context.DoctorSpecialities
                                 .Include(ds => ds.Doctor)
                                 .Include(ds => ds.Speciality)
                                 .ToListAsync();
        }

        public Task<DoctorSpeciality?> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}