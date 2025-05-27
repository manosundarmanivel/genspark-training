using Microsoft.AspNetCore.Mvc;
using FirstApi.Models;
using FirstApi.Services.Interfaces;

namespace FirstApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentController : ControllerBase
    {
        private readonly IService<Appointment> _service;
        public AppointmentController(IService<Appointment> service) => _service = service;

        [HttpGet]
        public ActionResult<List<Appointment>> GetAll() => _service.GetAll();

        [HttpPost]
        public IActionResult Add([FromBody] Appointment appointment)
        {
            _service.Add(appointment);
            return CreatedAtAction(nameof(GetAll), new { id = appointment.Id }, appointment);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _service.Delete(id);
            return NoContent();
        }
    }
}
