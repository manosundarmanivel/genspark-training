using Microsoft.AspNetCore.Mvc;
using FirstApi.Models.DTOs;
using FirstApi.Services.Interfaces;

namespace FirstApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DoctorController : ControllerBase
    {
        private readonly IDoctorService _doctorService;

        public DoctorController(IDoctorService doctorService)
        {
            _doctorService = doctorService;
        }

        [HttpGet]
        public async Task<ActionResult<List<DoctorDTO>>> GetAll()
        {
            var doctors = await _doctorService.GetAllAsync();
            return Ok(doctors);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DoctorDTO>> GetById(int id)
        {
            var doctor = await _doctorService.GetByIdAsync(id);
            if (doctor == null)
                return NotFound();

            return Ok(doctor);
        }

        [HttpGet("speciality")]
        public async Task<ActionResult<List<DoctorDTO>>> GetBySpeciality([FromQuery] string name)
        {
            var doctors = await _doctorService.GetBySpecialityAsync(name);
            return Ok(doctors);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] DoctorDTO doctor)
        {
            await _doctorService.AddAsync(doctor);
            return CreatedAtAction(nameof(GetById), new { id = doctor.Id }, doctor);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _doctorService.DeleteAsync(id);
            return NoContent();
        }
    }
}
