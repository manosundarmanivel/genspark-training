using Microsoft.AspNetCore.Mvc;
using FirstApi.Models;
using FirstApi.Services.Interfaces;

namespace FirstApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientController : ControllerBase
    {
        private readonly IService<Patient> _service;
        public PatientController(IService<Patient> service) => _service = service;

        [HttpGet]
        public ActionResult<List<Patient>> GetAll() => _service.GetAll();

        [HttpPost]
        public IActionResult Add([FromBody] Patient patient)
        {
            _service.Add(patient);
            return CreatedAtAction(nameof(GetAll), new { id = patient.Id }, patient);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _service.Delete(id);
            return NoContent();
        }
    }
}