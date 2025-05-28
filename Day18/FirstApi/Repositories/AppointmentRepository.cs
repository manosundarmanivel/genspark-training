using FirstApi.Models;
using FirstApi.Repositories.Interfaces;
using FirstApi.Data;
using Microsoft.EntityFrameworkCore;

namespace FirstApi.Repositories
{
    public class AppointmentRepository : IRepository<Appointment>
    {
        private readonly AppDbContext _context;

        public AppointmentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Appointment>> GetAllAsync()
        {
            return await _context.Appointments
                                 .Include(a => a.Doctor)
                                 .Include(a => a.Patient)
                                 .ToListAsync();
        }

        public async Task<Appointment?> GetByIdAsync(int id)
        {
            return await _context.Appointments
                                 .Include(a => a.Doctor)
                                 .Include(a => a.Patient)
                                 .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task AddAsync(Appointment appointment)
        {
            await _context.Appointments.AddAsync(appointment);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                _context.Appointments.Remove(appointment);
                await _context.SaveChangesAsync();
            }
        }
    }
}
