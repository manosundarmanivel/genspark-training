using DoctorAppointment.Interfaces;
using DoctorAppointment.Models;
using DoctorAppointment.Models.DTO;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace DoctorAppointment.Controllers 
{
    [ApiController]
    [Route("/api/[controller]")]
    [CustomExceptionFilter]   
    public class DoctorController : ControllerBase
    {
        private readonly IDoctorService _doctorService;

        public DoctorController(IDoctorService doctorService)
        {
            _doctorService = doctorService;
        }

        // [HttpGet]
        // public ActionResult<IEnumerable<Doctor>> GetDoctors()
        // {
        //     return Ok(doctors);
        // }
        [HttpPost]
        public async Task<ActionResult<Doctor>> PostDoctor([FromBody] DoctorAddDto doctor)
        {
            try
            {
                var newDoctor = await _doctorService.AddDoctor(doctor);
                if (newDoctor != null)
                    return Created("", newDoctor);
                return BadRequest("Unable to process request at this moment");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("by-speciality/{speciality}")]
        public async Task<ActionResult<IEnumerable<Doctor>>> GetDoctorsBySpeciality(string speciality)
        {
            try
            {
                var doctors = await _doctorService.GetDoctorsBySpeciality(speciality);
                if (doctors == null || !doctors.Any())
                    return NotFound("No doctors found with the given speciality");
                return Ok(doctors);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving doctors: {ex.Message}");
            }
        }

    }
}