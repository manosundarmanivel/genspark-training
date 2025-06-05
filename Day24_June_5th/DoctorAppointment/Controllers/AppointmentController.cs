using DoctorAppointment.Models;
using DoctorAppointment.Models.DTO;
using FirstAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DoctorAppointment.Interfaces;


namespace DoctorAppointment.Controllers
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

      
        [HttpPost]
        public async Task<ActionResult<Appointment>> AddAppointment([FromBody] AppointmentAddDto appointmentDto)
        {
            try
            {
                var appointment = await _appointmentService.AddAppointment(appointmentDto);
                return CreatedAtAction(nameof(GetAppointment), new { id = appointment.Id }, appointment);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Appointment>> GetAppointment(int id)
        {
            try
            {
                var appointment = await _appointmentService.GetAppointment(id);
                return Ok(appointment);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut("cancel/{appointmentId}/doctor/{doctorId}")]
        [Authorize(Policy = "ExperiencedDoctorOnly")]
        public async Task<IActionResult> CancelAppointment(int appointmentId, int doctorId)
        {
            try
            {
                var result = await _appointmentService.CancelAppointment(appointmentId, doctorId);
                if (result)
                    return Ok("Appointment cancelled successfully.");
                return BadRequest("Unable to cancel appointment.");
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
