using DoctorAppointment.Interfaces;
using DoctorAppointment.Models;
using DoctorAppointment.Models.DTO;
using DoctorAppointment.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace DoctorAppointment.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class PatientController : ControllerBase
    {
        private IPatientService _patser;
        public PatientController(IPatientService patser)
        {
            _patser = patser;
        }

        [HttpGet]
        [Authorize(Roles ="Patient")]
        public async Task<ActionResult<IEnumerable<Patient>>> GetAll()
        {
            try
            {
                var patients = await _patser.GetAll();
                return Ok(patients);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Patient>> Get(int id)
        {
            try
            {
                var patient = await _patser.Get(id);
                return Ok(patient);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        public async Task<ActionResult<Patient>> Add(PatientAddDto patient)
        {
            try
            {
                    // var patient1 = await Get(patient.Id);
                    var Patient = await _patser.Add(patient);
                    return Ok(Patient);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

  
}