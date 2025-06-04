using DoctorAppointment.Models;

namespace DoctorAppointment.Interfaces
{
    public interface IEncryptionService
    {
        public Task<EncryptModel> EncryptData(EncryptModel data);
    }
}