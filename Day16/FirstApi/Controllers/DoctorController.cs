using Microsoft.AspNetCore.Mvc;
using FirstApi.Models;
using FirstApi.Services.Interfaces;

namespace FirstApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DoctorController : ControllerBase
    {
        private readonly IService<Doctor> _service;
        public DoctorController(IService<Doctor> service) => _service = service;

        [HttpGet]
        public ActionResult<List<Doctor>> GetAll() => _service.GetAll();

        [HttpPost]
        public IActionResult Add([FromBody] Doctor doctor)
        {
            _service.Add(doctor);
            return CreatedAtAction(nameof(GetAll), new { id = doctor.Id }, doctor);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _service.Delete(id);
            return NoContent();
        }
    }
}
