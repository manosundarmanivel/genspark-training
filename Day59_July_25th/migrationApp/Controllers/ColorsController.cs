using ChienVHShopOnline.Dtos;
using ChienVHShopOnline.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChienVHShopOnline.ApiControllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ColorsController : ControllerBase
    {
        private readonly IColorService _colorService;

        public ColorsController(IColorService colorService)
        {
            _colorService = colorService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var colors = await _colorService.GetAllAsync();
            return Ok(colors);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var color = await _colorService.GetByIdAsync(id);
            if (color == null) return NotFound();
            return Ok(color);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateColorDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var created = await _colorService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.ColorId }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateColorDto dto)
        {
            if (id != dto.ColorId) return BadRequest("ID mismatch");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var updated = await _colorService.UpdateAsync(dto);
            if (updated == null) return NotFound();

            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _colorService.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
