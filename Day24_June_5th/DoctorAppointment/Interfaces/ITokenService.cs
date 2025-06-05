using DoctorAppointment.Models;

namespace DoctorAppointment.Interfaces
{
    public interface ITokenService
    {
        public Task<string> GenerateToken(User user);
    }
}