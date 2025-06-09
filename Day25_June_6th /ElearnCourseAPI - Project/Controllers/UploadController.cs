using ElearnAPI.DTOs;
using ElearnAPI.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;

namespace ElearnAPI.Controllers
{
    [ApiController]
    [Route("api/v1/files")]
    public class UploadController : ControllerBase
    {
        private readonly IUploadService _uploadService;

        public UploadController(IUploadService uploadService)
        {
            _uploadService = uploadService;
        }

        [Authorize(Roles = "Instructor")]
        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm] UploadFileDto dto)
        {
            var path = Path.Combine("Uploads", dto.File.FileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await dto.File.CopyToAsync(stream);
            }

            var fileMeta = await _uploadService.UploadFileAsync(new UploadedFileDto
            {
                FileName = dto.File.FileName,
                Path = path,
                UploadedBy = User.Identity?.Name ?? "Unknown"
            });

            return Ok(new { success = true, data = fileMeta });
        }

        [HttpGet("{filename}")]
        public IActionResult Download(string filename)
        {
            var path = Path.Combine("Uploads", filename);
            if (!System.IO.File.Exists(path))
                return NotFound(new { success = false, message = "File not found" });

            var fileBytes = System.IO.File.ReadAllBytes(path);
            return File(fileBytes, "application/octet-stream", filename);
        }
    }
}
