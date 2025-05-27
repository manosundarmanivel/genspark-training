using Microsoft.AspNetCore.Mvc;
using FirstApi.Models; 

[ApiController]
[Route("/api/[controller]")]
public class DoctorController : ControllerBase
{
    public static List<Doctor> doctors = new List<Doctor>
    {
        new Doctor { Id = 1, Name = "Dr. Smith", Specialization = "Cardiology", PhoneNumber = "123-456-7890", Email = "dr.smith@example.com" },
        new Doctor { Id = 2, Name = "Dr. Johnson", Specialization = "Neurology", PhoneNumber = "987-654-3210", Email = "dr.johnson@example.com" }
    };

    [HttpGet]
    [ProducesResponseType(typeof(List<Doctor>), StatusCodes.Status200OK)]
    public ActionResult<List<Doctor>> GetDoctors()
    {
        return Ok(doctors);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(Doctor), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public ActionResult<Doctor> GetDoctorById(int id)
    {
        var doctor = doctors.FirstOrDefault(d => d.Id == id);
        if (doctor == null)
            return NotFound("Doctor not found.");

        return Ok(doctor);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Doctor), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public ActionResult<Doctor> AddDoctor([FromBody] Doctor doctor)
    {
        if (doctor == null || string.IsNullOrEmpty(doctor.Name) || string.IsNullOrEmpty(doctor.Specialization))
        {
            return BadRequest("Invalid doctor data.");
        }

        doctor.Id = doctors.Max(d => d.Id) + 1;
        doctors.Add(doctor);
        return CreatedAtAction(nameof(GetDoctorById), new { id = doctor.Id }, doctor);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public ActionResult DeleteDoctor(int id)
    {
        var doctor = doctors.FirstOrDefault(d => d.Id == id);
        if (doctor == null)
        {
            return NotFound("Doctor not found.");
        }

        doctors.Remove(doctor);
        return NoContent();
    }
}
