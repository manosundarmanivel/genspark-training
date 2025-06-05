using NUnit.Framework;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DoctorAppointment.Interfaces;
using DoctorAppointment.Models;
using DoctorAppointment.Service;
using AutoMapper;

namespace DoctorAppointment.Test
{
    public class DoctorServiceTests
    {
        private Mock<IRepository<int, Doctor>> _doctorRepoMock;
        private Mock<IRepository<int, Speciality>> _specialityRepoMock;
        private Mock<IRepository<int, DoctorSpeciality>> _doctorSpecialityRepoMock;
        private Mock<IRepository<string, User>> _userRepoMock;
        private Mock<IEncryptionService> _encryptionServiceMock;
        private Mock<IMapper> _mapperMock;
        private DoctorService _doctorService;

        [SetUp]
        public void Setup()
        {
            _doctorRepoMock = new Mock<IRepository<int, Doctor>>();
            _specialityRepoMock = new Mock<IRepository<int, Speciality>>();
            _doctorSpecialityRepoMock = new Mock<IRepository<int, DoctorSpeciality>>();
            _userRepoMock = new Mock<IRepository<string, User>>();
            _encryptionServiceMock = new Mock<IEncryptionService>();
            _mapperMock = new Mock<IMapper>();

            _doctorService = new DoctorService(
                _doctorRepoMock.Object,
                _specialityRepoMock.Object,
                _doctorSpecialityRepoMock.Object,
                _userRepoMock.Object,
                _encryptionServiceMock.Object,
                _mapperMock.Object
            );
        }

        [Test]
        public async Task GetDoctorsBySpeciality_ReturnsCorrectDoctors()
        {
            // Arrange
            var specialityName = "Cardiology";

            var doctors = new List<Doctor>
            {
                new Doctor
                {
                    Id = 1,
                    Name = "Dr. Smith"
                },
                new Doctor
                {
                    Id = 2,
                    Name = "Dr. Adams"
                }
            };

            var specialities = new List<Speciality>
            {
                new Speciality { Id = 1, Name = "Cardiology" },
                new Speciality { Id = 2, Name = "Neurology" }
            };

            var doctorSpecialities = new List<DoctorSpeciality>
            {
                new DoctorSpeciality { DoctorId = 1, SpecialityId = 1 },
                new DoctorSpeciality { DoctorId = 2, SpecialityId = 2 }
            };

            _specialityRepoMock.Setup(repo => repo.GetAll()).ReturnsAsync(specialities);
            _doctorSpecialityRepoMock.Setup(repo => repo.GetAll()).ReturnsAsync(doctorSpecialities);
            _doctorRepoMock.Setup(repo => repo.GetAll()).ReturnsAsync(doctors);

            // Act
            var result = await _doctorService.GetDoctorsBySpeciality(specialityName);

            // Assert
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result.First().Name, Is.EqualTo("Dr. Smith"));
        }


        
    }
}
