using DoctorAppointment.Contexts;
using DoctorAppointment.Interfaces;
using DoctorAppointment.Models;
using DoctorAppointment.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FirstAPI.Repositories
{
    public class UserRepository : Repository<string, User>
    {
        public UserRepository(ClinicContext context):base(context)
        {
            
        }
        public override async Task<User> Get(string key)
        {
            return await _clinicContext.Users.SingleOrDefaultAsync(u => u.Username == key);
        }

        public override async Task<IEnumerable<User>> GetAll()
        {
            return await _clinicContext.Users.ToListAsync();
        }
            
    }
}