using ChienVHShopOnline.Dtos;
using ChienVHShopOnline.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChienVHShopOnline.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NewsController : ControllerBase
    {
        private readonly INewsService _service;

        public NewsController(INewsService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var news = await _service.GetAllAsync();
            return Ok(news);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var news = await _service.GetByIdAsync(id);
            return news == null ? NotFound() : Ok(news);
        }

[HttpPost]
[Consumes("multipart/form-data")]
public async Task<IActionResult> Create([FromForm] CreateNewsDto dto)
{
    if (!ModelState.IsValid) return BadRequest(ModelState);

    string? filePath = null;

    if (dto.ImageFile != null && dto.ImageFile.Length > 0)
    {
        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/news-images");
        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.ImageFile.FileName);
        var fullPath = Path.Combine(uploadsFolder, fileName);

        using (var stream = new FileStream(fullPath, FileMode.Create))
        {
            await dto.ImageFile.CopyToAsync(stream);
        }

        filePath = "/news-images/" + fileName;
    }

    dto.Image = filePath; 
    dto.ImageFile = null; 

    var created = await _service.CreateAsync(dto);
    return CreatedAtAction(nameof(Get), new { id = created.NewsId }, created);
}


        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateNewsDto dto)
        {
            var success = await _service.UpdateAsync(id, dto);
            return success ? NoContent() : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);
            return deleted ? NoContent() : NotFound();
        }
    }
}
