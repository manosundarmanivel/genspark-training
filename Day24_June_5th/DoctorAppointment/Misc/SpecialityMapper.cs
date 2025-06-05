using DoctorAppointment.Models;
using DoctorAppointment.Models.DTO;


namespace DoctorAppointment.Misc
{
    public class SpecialityMapper
    {
        public Speciality? MapSpecialityAddRequestDoctor(SpecialityAddDto addRequestDto)
        {
            Speciality speciality = new();
            speciality.Name = addRequestDto.Name;
            return speciality;
        }

        public DoctorSpeciality MapDoctorSpecility(int doctorId, int specialityId)
        {
            DoctorSpeciality doctorSpeciality = new();
            doctorSpeciality.DoctorId = doctorId;
            doctorSpeciality.SpecialityId = specialityId;
            return doctorSpeciality;
        }
    }
}