using Microsoft.AspNetCore.Mvc;
using FirstApi.Models;
using FirstApi.Services.Interfaces;

namespace FirstApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;

        public AppointmentController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Appointment>>> GetAll()
        {
            var appointments = await _appointmentService.GetAllAsync();
            return Ok(appointments);
        }

        [HttpGet("doctor/{doctorId}")]
        public async Task<ActionResult<List<Appointment>>> GetByDoctorId(int doctorId)
        {
            var appointments = await _appointmentService.GetByDoctorIdAsync(doctorId);
            return Ok(appointments);
        }

        [HttpGet("date")]
        public async Task<ActionResult<List<Appointment>>> GetByDate([FromQuery] DateTime date)
        {
            var appointments = await _appointmentService.GetByDateAsync(date);
            return Ok(appointments);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Appointment appointment)
        {
            await _appointmentService.AddAsync(appointment);
            return CreatedAtAction(nameof(GetAll), new { id = appointment.Id }, appointment);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _appointmentService.DeleteAsync(id);
            return NoContent();
        }
    }
}
