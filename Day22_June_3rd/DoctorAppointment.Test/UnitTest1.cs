using DoctorAppointment.Contexts;
using DoctorAppointment.Models;
using DoctorAppointment.Repositories;
using DoctorAppointment.Interfaces;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Text;
using System.Threading.Tasks;

namespace DoctorAppointment.Test;

public class Tests
{
    private ClinicContext _context;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<ClinicContext>()
                            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // ensure fresh DB per test
                            .Options;
        _context = new ClinicContext(options);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }

    [Test]
    public async Task AddDoctorTest()
    {
        // Arrange
        var email = "test@gmail.com";
        var password = Encoding.UTF8.GetBytes("test123");
        var key = Guid.NewGuid().ToByteArray();
        var user = new User
        {
            Username = email,
            Password = password,
            HashKey = key,
            Role = "Doctor"
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var doctor = new Doctor
        {
            Name = "Dr. Test",
            YearsOfExperience = 5,
            Email = email
        };

        IRepository<int, Doctor> doctorRepo = new DoctorRepo(_context);

        // Act
        var result = await doctorRepo.Add(doctor);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.GreaterThan(0));
    }

    [Test]
    public async Task GetDoctorSuccessTest()
    {
        // Arrange
        var email = "test2@gmail.com";
        var password = Encoding.UTF8.GetBytes("test456");
        var key = Guid.NewGuid().ToByteArray();
        var user = new User
        {
            Username = email,
            Password = password,
            HashKey = key,
            Role = "Doctor"
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var doctor = new Doctor
        {
            Name = "Dr. Success",
            YearsOfExperience = 3,
            Email = email
        };

        IRepository<int, Doctor> doctorRepo = new DoctorRepo(_context);
        var addedDoctor = await doctorRepo.Add(doctor);

        // Act
        var result = await doctorRepo.Get(addedDoctor.Id);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(addedDoctor.Id));
        Assert.That(result.Name, Is.EqualTo("Dr. Success"));
    }

    [Test]
    public async Task GetDoctorExceptionTest()
    {
        // Arrange
        var email = "test3@gmail.com";
        var password = Encoding.UTF8.GetBytes("test789");
        var key = Guid.NewGuid().ToByteArray();
        var user = new User
        {
            Username = email,
            Password = password,
            HashKey = key,
            Role = "Doctor"
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var doctor = new Doctor
        {
            Name = "Dr. NotFound",
            YearsOfExperience = 7,
            Email = email
        };

        IRepository<int, Doctor> doctorRepo = new DoctorRepo(_context);
        await doctorRepo.Add(doctor);

        int nonExistingId = 999; // Intentionally invalid

        // Act & Assert
        var ex = Assert.ThrowsAsync<Exception>(async () => await doctorRepo.Get(nonExistingId));
        Assert.That(ex.Message, Is.EqualTo("No Doctor with the given ID")); // Make sure your repository matches this message
    }
}
