using Microsoft.AspNetCore.Mvc;
using SecondWebApi.Interfaces;
using SecondWebApi.Models;
using SecondWebApi.Models.Dtos;

namespace SecondWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientController : ControllerBase
    {
        private readonly IPatientService _patientService;

        public PatientController(IPatientService patientService)
        {
            _patientService = patientService;
        }

       
        [HttpPost]
        public async Task<IActionResult> AddPatient([FromBody] PatientAddDto patientAddDto)
        {
            if (patientAddDto == null)
                return BadRequest("Patient data is required.");

            var addedPatient = await _patientService.AddPatientAsync(patientAddDto);

            if (addedPatient == null)
                return StatusCode(500, "An error occurred while adding the patient.");

            return CreatedAtAction(nameof(GetPatientByName), new { name = addedPatient.Name }, addedPatient);
        }

        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePatient(int id)
        {
            try
            {
                await _patientService.DeletePatient(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error deleting patient with ID {id}: {ex.Message}");
            }
        }

       
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePatient(int id, [FromBody] Patient patient)
        {
            if (patient == null)
                return BadRequest("Patient data is required.");

            var updatedPatient = await _patientService.UpdatePatient(id, patient);

            if (updatedPatient == null)
                return StatusCode(500, $"Error updating patient with ID {id}.");

            return Ok(updatedPatient);
        }

       
        [HttpGet("name/{name}")]
        public async Task<IActionResult> GetPatientByName(string name)
        {
            var patients = await _patientService.GetPatientByName(name);

            if (patients == null || !patients.Any())
                return NotFound($"No patients found with the name '{name}'.");

            return Ok(patients);
        }
    }
}
