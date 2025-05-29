using Microsoft.AspNetCore.Mvc;
using SecondWebApi.Interfaces;
using SecondWebApi.Models;
using SecondWebApi.Models.Dtos;

[ApiController]
[Route("api/[controller]")]
public class DoctorController : ControllerBase
{
    private readonly IDoctorService _doctorService;
    public DoctorController(IDoctorService doctorService)
    {
        _doctorService = doctorService;
    }

    [HttpPost]
    public async Task<IActionResult> AddDoctor([FromBody] DoctorAddDto doctorAddDto)
    {
        var result = await _doctorService.AddDoctor(doctorAddDto);
        if (result == null)
            return BadRequest("Failed to add doctor.");
        return CreatedAtAction(nameof(GetDoctorByName), new { name = result.Name }, result);
    }

    [HttpGet("by-name/{name}")]
    public async Task<IActionResult> GetDoctorByName(string name)
    {
        var doctor = await _doctorService.GetDoctorByName(name);
        if (doctor == null)
            return NotFound($"Doctor with name '{name}' not found.");
        return Ok(doctor);
    }

    [HttpGet("by-speciality/{specialityName}")]
    public async Task<IActionResult> GetDoctorsBySpeciality(string specialityName)
    {
        var doctors = await _doctorService.GetDoctorsBySpeciality(specialityName);
        if (doctors == null || !doctors.Any())
            return NotFound($"No doctors found for speciality '{specialityName}'.");
        return Ok(doctors);
    }
}