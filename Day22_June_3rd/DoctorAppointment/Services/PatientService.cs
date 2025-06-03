using System.Net.Http.Headers;
using DoctorAppointment.Contexts;
using DoctorAppointment.Interfaces;
using DoctorAppointment.Models;
using DoctorAppointment.Models.DTO;
using DoctorAppointment.Repositories;
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
    public class PatientServices : IPatientService
    {

         private readonly IEncryptionService _encryptionService;
        private readonly IMapper _mapper;
        private readonly IRepository<string, User> _userRepository;
        private PatientRepo _repo;
        public PatientServices(PatientRepo repo,
          IRepository<string, User> userRepository,
                            IEncryptionService encryptionService,
                            IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
            _userRepository = userRepository;
            _encryptionService = encryptionService;


        }
        public async Task<Patient> Add(PatientAddDto patient)
        {
            try
            {
                var user = _mapper.Map<PatientAddDto, User>(patient);
                var encryptedData = await _encryptionService.EncryptData(new EncryptModel
                {
                    Data= patient.Password
                });
                user.Password = encryptedData.EncryptedData;
                user.HashKey = encryptedData.HashKey;
                user.Role = "Patient";
                user = await _userRepository.Add(user);


                var newpatient = new Patient
                {
                    Name = patient.Name,
                    Age = patient.Age,
                    Email = patient.Email,
                    Phone = patient.Phone,
                    User = user,
                    Appointments = null
                };
                newpatient = await _repo.Add(newpatient);
                if (newpatient == null)
                    throw new Exception("Could not add Patient");
            
               
                return newpatient;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            

        }

        public async Task<Patient> Delete(int key)
        {
            var patient =  await _repo.Delete(key);
            return patient;
        }

        public async Task<Patient> Get(int key)
        {
            var patient =  await _repo.Get(key);
            return patient;
        }

        public async Task<IEnumerable<Patient>> GetAll()
        {
            return await _repo.GetAll();
        }

        public async Task<Patient> Update(int key, Patient item)
        {
            return await _repo.Update(key, item);
        }
    }
}