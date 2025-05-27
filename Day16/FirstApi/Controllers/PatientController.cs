using Microsoft.AspNetCore.Mvc;
using FirstApi.Models; 

[ApiController]
[Route("/api/[controller]")]
public class PatientController : ControllerBase
{
    public static List<Patient> patients = new List<Patient>
    {
        new Patient { Id = 1, Name = "John Doe", Age = 30, Gender = "Male", Address = "123 Main St" },
    };

    [HttpGet]
    [ProducesResponseType(typeof(List<Patient>), StatusCodes.Status200OK)]
    public ActionResult<List<Patient>> GetPatients()
    {
        return Ok(patients);
    }
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Patient), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public ActionResult<Patient> GetPatientById(int id)
    {
        var patient = patients.FirstOrDefault(p => p.Id == id);
        if (patient == null)
            return NotFound("Patient not found.");

        return Ok(patient);
    }
    [HttpPost]
    [ProducesResponseType(typeof(Patient), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public ActionResult<Patient> CreatePatient([FromBody] Patient newPatient)
    {
        if (newPatient == null)
            return BadRequest("Invalid patient data.");

        newPatient.Id = patients.Max(p => p.Id) + 1;
        patients.Add(newPatient);
        return CreatedAtAction(nameof(GetPatientById), new { id = newPatient.Id }, newPatient);
    }
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(Patient), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public ActionResult<Patient> UpdatePatient(int id, [FromBody] Patient updatedPatient)
    {
        var patient = patients.FirstOrDefault(p => p.Id == id);
        if (patient == null)
            return NotFound("Patient not found.");

        patient.Name = updatedPatient.Name;
        patient.Age = updatedPatient.Age;
        patient.Gender = updatedPatient.Gender;
        patient.Address = updatedPatient.Address;

        return Ok(patient);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public ActionResult DeletePatient(int id)
    {
        var patient = patients.FirstOrDefault(p => p.Id == id);
        if (patient == null)
            return NotFound("Patient not found.");

        patients.Remove(patient);
        return NoContent();
    }
}