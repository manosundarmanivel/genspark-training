using NUnit.Framework;
using Moq;
using System.Threading.Tasks;
using DoctorAppointment.Service;
using DoctorAppointment.Interfaces;
using DoctorAppointment.Models;
using DoctorAppointment.Models.DTO;
using DoctorAppointment.Repositories;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;

namespace DoctorAppointment.Test
{
    public class PatientServiceTests
    {
        private Mock<PatientRepo> _patientRepoMock;
        private Mock<IRepository<string, User>> _userRepoMock;
        private Mock<IEncryptionService> _encryptionServiceMock;
        private Mock<IMapper> _mapperMock;
        private PatientServices _patientService;

        [SetUp]
        public void Setup()
        {
            _patientRepoMock = new Mock<PatientRepo>(null); 
            _userRepoMock = new Mock<IRepository<string, User>>();
            _encryptionServiceMock = new Mock<IEncryptionService>();
            _mapperMock = new Mock<IMapper>();

            _patientService = new PatientServices(
                _patientRepoMock.Object,
                _userRepoMock.Object,
                _encryptionServiceMock.Object,
                _mapperMock.Object
            );
        }

        [Test]
        public async Task AddPatient_Success_ReturnsPatient()
        {
            // Arrange
            var patientDto = new PatientAddDto
            {
                Name = "John Doe",
                Age = 30,
                Email = "john@example.com",
                Phone = "1234567890",
                Password = "securePass"
            };

            var encryptedData = new EncryptModel
            {
                Data = patientDto.Password,
                EncryptedData = new byte[] { 1, 2, 3 },
                HashKey = new byte[] { 4, 5, 6 }
            };

            var user = new User
            {
                Username = patientDto.Email,
                Password = encryptedData.EncryptedData,
                HashKey = encryptedData.HashKey,
                Role = "Patient"
            };

            var expectedPatient = new Patient
            {
                Id = 1,
                Name = patientDto.Name,
                Age = patientDto.Age,
                Email = patientDto.Email,
                Phone = patientDto.Phone,
                User = user
            };

            _mapperMock.Setup(m => m.Map<PatientAddDto, User>(It.IsAny<PatientAddDto>())).Returns(user);
            _encryptionServiceMock.Setup(e => e.EncryptData(It.IsAny<EncryptModel>())).ReturnsAsync(encryptedData);
            _userRepoMock.Setup(u => u.Add(It.IsAny<User>())).ReturnsAsync(user);
            _patientRepoMock.Setup(p => p.Add(It.IsAny<Patient>())).ReturnsAsync(expectedPatient);

            // Act
            var result = await _patientService.Add(patientDto);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo("John Doe"));
            Assert.That(result.Email, Is.EqualTo("john@example.com"));
        }

        [Test]
        public async Task GetPatient_Success_ReturnsPatient()
        {
            var patient = new Patient
            {
                Id = 1,
                Name = "Jane Doe",
                Age = 28,
                Email = "jane@example.com"
            };

            _patientRepoMock.Setup(r => r.Get(1)).ReturnsAsync(patient);

            var result = await _patientService.Get(1);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(1));
            Assert.That(result.Name, Is.EqualTo("Jane Doe"));
        }

        [Test]
        public async Task DeletePatient_Success_ReturnsDeletedPatient()
        {
            var patient = new Patient { Id = 2, Name = "Mark", Email = "mark@example.com" };
            _patientRepoMock.Setup(r => r.Delete(2)).ReturnsAsync(patient);

            var result = await _patientService.Delete(2);

            Assert.That(result.Id, Is.EqualTo(2));
            Assert.That(result.Name, Is.EqualTo("Mark"));
        }

        [Test]
        public async Task GetAllPatients_Success_ReturnsPatients()
        {
            var patients = new List<Patient>
            {
                new Patient { Id = 1, Name = "Patient A" },
                new Patient { Id = 2, Name = "Patient B" }
            };

            _patientRepoMock.Setup(r => r.GetAll()).ReturnsAsync(patients);

            var result = await _patientService.GetAll();

            Assert.That(result.Count(), Is.EqualTo(2));
        }
    }
}
