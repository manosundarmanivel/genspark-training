
using DoctorAppointment.Models.DTO;
using DoctorAppointment.Models.DTOs.DoctorSpecialities;

namespace DoctorAppointment.Interfaces
{
    public interface IAuthenticationService
    {
        public Task<UserLoginResponse> Login(UserLoginRequest user);
    }
}