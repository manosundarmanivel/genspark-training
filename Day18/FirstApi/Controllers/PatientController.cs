using Microsoft.AspNetCore.Mvc;
using FirstApi.Models;
using FirstApi.Models.DTOs;
using FirstApi.Services.Interfaces;

namespace FirstApi.Controllers
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

     
        [HttpGet]
        public async Task<ActionResult<List<PatientDTO>>> GetAll()
        {
            var patients = await _patientService.GetAllAsync();
            return Ok(patients);
        }

        
        [HttpGet("{id}/appointments")]
        public async Task<ActionResult<List<Appointment>>> GetAppointments(int id)
        {
            var appointments = await _patientService.GetAppointmentsAsync(id);
            return Ok(appointments);
        }

       
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] PatientDTO patientDto)
        {
            await _patientService.AddAsync(patientDto);
            return CreatedAtAction(nameof(GetAll), new { name = patientDto.Name }, patientDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _patientService.DeleteAsync(id);
            return NoContent();
        }
    }
}
