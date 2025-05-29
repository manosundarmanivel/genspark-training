using Microsoft.AspNetCore.Mvc;
using SecondWebApi.Interfaces;
using SecondWebApi.Models.Dtos;
using SecondWebApi.Models;

[ApiController]
[Route("api/[controller]")]
public class SpecialityController : ControllerBase
{
    private readonly ISpecialityService _specialityService;

    public SpecialityController(ISpecialityService specialityService)
    {
        _specialityService = specialityService;
    }

    [HttpPost]
    public async Task<IActionResult> AddSpeciality([FromBody] SpecialityAddDto specialityAddDto)
    {
        var result = await _specialityService.AddSpeciality(specialityAddDto);
        if (result == null)
        {
            return BadRequest("Failed to add speciality.");
        }
        return CreatedAtAction(nameof(AddSpeciality), new { id = result.Id }, result);
    }

    [HttpGet]
    public async Task<IActionResult> GetSpecialities()
    {
        var result = await _specialityService.GetSpecialities();
        if (result == null || !result.Any())
        {
            return NotFound("No specialities found.");
        }
        return Ok(result);
    }
}
