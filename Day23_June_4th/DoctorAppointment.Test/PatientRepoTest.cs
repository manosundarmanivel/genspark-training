using DoctorAppointment.Contexts;
using DoctorAppointment.Interfaces;
using DoctorAppointment.Models;
using DoctorAppointment.Repositories;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorAppointment.Test
{
    public class PatientRepoTests
    {
        private ClinicContext _context;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ClinicContext>()
                            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                            .Options;
            _context = new ClinicContext(options);
        }

        [TearDown]
        public void Cleanup()
        {
            _context.Dispose();
        }

        [Test]
        public async Task AddPatientTest()
        {
            // Arrange
            var email = "patient1@gmail.com";
            var password = Encoding.UTF8.GetBytes("secret123");
            var key = Guid.NewGuid().ToByteArray();
            var user = new User
            {
                Username = email,
                Password = password,
                HashKey = key,
                Role = "Patient"
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var patient = new Patient
            {
                Name = "John Doe",
                Age = 30,
                Email = email,
                Phone = "1234567890"
            };

            IRepository<int, Patient> patientRepo = new PatientRepo(_context);

            // Act
            var result = await patientRepo.Add(patient);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.GreaterThan(0));
            Assert.That(result.Email, Is.EqualTo(email));
        }

        [Test]
        public async Task GetPatientSuccessTest()
        {
            // Arrange
            var email = "patient2@gmail.com";
            var user = new User
            {
                Username = email,
                Password = Encoding.UTF8.GetBytes("pass123"),
                HashKey = Guid.NewGuid().ToByteArray(),
                Role = "Patient"
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var patient = new Patient
            {
                Name = "Jane Smith",
                Age = 40,
                Email = email,
                Phone = "9876543210"
            };

            IRepository<int, Patient> patientRepo = new PatientRepo(_context);
            var addedPatient = await patientRepo.Add(patient);

            // Act
            var result = await patientRepo.Get(addedPatient.Id);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(addedPatient.Id));
            Assert.That(result.Name, Is.EqualTo("Jane Smith"));
        }

        [Test]
        public void GetPatientExceptionTest()
        {
            // Arrange
            IRepository<int, Patient> patientRepo = new PatientRepo(_context);
            int nonExistingId = 999;

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () =>
            {
                await patientRepo.Get(nonExistingId);
            });

            Assert.That(ex.Message, Is.EqualTo("No patient with the given ID"));
        }

        [Test]
        public async Task GetAllPatientsTest()
        {
            // Arrange
            var user1 = new User
            {
                Username = "user1@gmail.com",
                Password = Encoding.UTF8.GetBytes("pass1"),
                HashKey = Guid.NewGuid().ToByteArray(),
                Role = "Patient"
            };
            var user2 = new User
            {
                Username = "user2@gmail.com",
                Password = Encoding.UTF8.GetBytes("pass2"),
                HashKey = Guid.NewGuid().ToByteArray(),
                Role = "Patient"
            };

            _context.Users.AddRange(user1, user2);
            await _context.SaveChangesAsync();

            var patient1 = new Patient { Name = "Alice", Age = 25, Email = user1.Username, Phone = "1112223333" };
            var patient2 = new Patient { Name = "Bob", Age = 35, Email = user2.Username, Phone = "4445556666" };

            var repo = new PatientRepo(_context);
            await repo.Add(patient1);
            await repo.Add(patient2);

            // Act
            var result = await repo.GetAll();

            // Assert
            Assert.That(result.Count(), Is.EqualTo(2));
        }

        [Test]
        public void GetAllPatientsEmptyExceptionTest()
        {
            // Arrange
            IRepository<int, Patient> repo = new PatientRepo(_context);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () =>
            {
                await repo.GetAll();
            });

            Assert.That(ex.Message, Is.EqualTo("No Patients in the database"));
        }
    }
}
