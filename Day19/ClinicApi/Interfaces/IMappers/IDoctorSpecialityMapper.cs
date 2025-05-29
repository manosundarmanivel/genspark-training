using SecondWebApi.Models;

namespace SecondWebApi.Interfaces.Mappers
{
    public interface IDoctorSpecialityMapper
    {
        IEnumerable<DoctorSpeciality> MapFromDoctorAndSpecialities(int doctorId, IEnumerable<Speciality> specialities);
    }
}
