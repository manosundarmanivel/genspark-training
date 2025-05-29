using SecondWebApi.Interfaces.Mappers;
using SecondWebApi.Models;

namespace SecondWebApi.Services.Mappers
{
    public class DoctorSpecialityMapper : IDoctorSpecialityMapper
    {
        public IEnumerable<DoctorSpeciality> MapFromDoctorAndSpecialities(int doctorId, IEnumerable<Speciality> specialities)
        {
            return specialities.Select(s => new DoctorSpeciality
            {
                DoctorId = doctorId,
                SpecialityId = s.Id
            });
        }
    }
}
