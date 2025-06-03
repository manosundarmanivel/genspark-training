using DoctorAppointment.Interfaces;
using DoctorAppointment.Models;
using DoctorAppointment.Models.DTO;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace DoctorAppointment.Controllers 
{
    [ApiController]
    [Route("api/[controller]")]
    public class SpecialityController :ControllerBase
    {
        private ISpecialityServices _ser;
        public SpecialityController(ISpecialityServices ser)
        {
            _ser = ser;
        }
        
        [HttpPost]
        public async Task<ActionResult<Speciality>> Add(SpecialityAddDto speciality)
        {
            try
            {
                var s = await _ser.AddSpeciality(speciality);
                return Ok(s);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Speciality>>> Getall()
        {
            try
            {
                var s = await _ser.GetSpecialities();
                return Ok(s);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}