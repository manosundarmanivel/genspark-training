using System.Net.Http.Headers;
using AutoMapper;
using DoctorAppointment.Contexts;
using DoctorAppointment.Interfaces;
using DoctorAppointment.Misc;
using DoctorAppointment.Models;
using DoctorAppointment.Models.DTO;
using DoctorAppointment.Repositories;
using DoctorAppointment.Services;
using FirstAPI.Repositories;
using Microsoft.OpenApi.Validations;

namespace DoctorAppointment.Service
{

  
    public class DoctorService : IDoctorService
    {
        DoctorMapper doctorMapper ;
        SpecialityMapper specialityMapper;
        private readonly IRepository<int, Doctor> _doctorRepository;
        private readonly IRepository<int, Speciality> _specialityRepository;
        private readonly IRepository<int, DoctorSpeciality> _doctorSpecialityRepository;
         private readonly IEncryptionService _encryptionService;
        private readonly IMapper _mapper;
        private readonly IRepository<string, User> _userRepository;
        public DoctorService(IRepository<int, Doctor> doctorRepository,
                            IRepository<int, Speciality> specialityRepository,
                            IRepository<int, DoctorSpeciality> doctorSpecialityRepository,

                            IRepository<string, User> userRepository,
                            IEncryptionService encryptionService,
                            IMapper mapper)
        {
            doctorMapper = new DoctorMapper();
            specialityMapper = new();
            _doctorRepository = doctorRepository;
            _specialityRepository = specialityRepository;
            _doctorSpecialityRepository = doctorSpecialityRepository;
            _encryptionService = encryptionService;
            _mapper = mapper;
            _userRepository = userRepository;

        }

        public async Task<Doctor> AddDoctor(DoctorAddDto doctor)
        {
            try
            {
                var user = _mapper.Map<DoctorAddDto, User>(doctor);
                var encryptedData = await _encryptionService.EncryptData(new EncryptModel
                {
                    Data= doctor.Password
                });
                user.Password = encryptedData.EncryptedData;
                user.HashKey = encryptedData.HashKey;
                user.Role = "Doctor";
                user = await _userRepository.Add(user);
                
                
                var newDoctor = doctorMapper.MapDoctorAddRequestDoctor(doctor);
                newDoctor = await _doctorRepository.Add(newDoctor);
                if (newDoctor == null)
                    throw new Exception("Could not add doctor");
                if (doctor.Specialities.Count() > 0)
                {
                    int[] specialities = await MapAndAddSpeciality(doctor);
                    for (int i = 0; i < specialities.Length; i++)
                    {
                        var doctorSpeciality = specialityMapper.MapDoctorSpecility(newDoctor.Id, specialities[i]);
                        doctorSpeciality = await _doctorSpecialityRepository.Add(doctorSpeciality);
                    }
                }
                return newDoctor;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            
        }

        private async Task<int[]> MapAndAddSpeciality(DoctorAddDto doctor)
        {
            int[] specialityIds = new int[doctor.Specialities.Count()];
            IEnumerable<Speciality> existingSpecialities = null;
            try
            {
                existingSpecialities = await _specialityRepository.GetAll();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            int count = 0;
            foreach (var item in doctor.Specialities)
            {
                Speciality speciality = null;
                if (existingSpecialities != null)
                    speciality = existingSpecialities.FirstOrDefault(s => s.Name.ToLower() == item.Name.ToLower());
                if (speciality == null)
                {
                    speciality = specialityMapper.MapSpecialityAddRequestDoctor(item);
                    speciality = await _specialityRepository.Add(speciality);
                }
                specialityIds[count] = speciality.Id;
                count++;
            }
            return specialityIds;
        }

        public Task<Doctor> GetDoctByName(string name)
        {
            throw new NotImplementedException();
        }

       public async Task<ICollection<Doctor>> GetDoctorsBySpeciality(string speciality)
{
    try
    {
        // Step 1: Get all specialities
        var specialities = await _specialityRepository.GetAll();
        var matchedSpeciality = specialities.FirstOrDefault(s => s.Name.ToLower() == speciality.ToLower());

        if (matchedSpeciality == null)
            throw new Exception("Speciality not found");

        // Step 2: Get all doctor-speciality relations
        var doctorSpecialities = await _doctorSpecialityRepository.GetAll();
        var doctorIds = doctorSpecialities
                        .Where(ds => ds.SpecialityId == matchedSpeciality.Id)
                        .Select(ds => ds.DoctorId)
                        .Distinct()
                        .ToList();

        if (doctorIds.Count == 0)
            return new List<Doctor>();

        // Step 3: Get all doctors and filter those by the above IDs
        var allDoctors = await _doctorRepository.GetAll();
        var filteredDoctors = allDoctors
                                .Where(d => doctorIds.Contains(d.Id))
                                .ToList();

        return filteredDoctors;
    }
    catch (Exception ex)
    {
        throw new Exception("Error fetching doctors by speciality: " + ex.Message);
    }
}

    }
}