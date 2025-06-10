using ElearnAPI.DTOs;
using ElearnAPI.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ElearnAPI.Controllers
{
    [ApiController]
    [Route("api/v1/files")]
    public class UploadController : ControllerBase
    {
        private readonly IUploadService _uploadService;
        private readonly string _uploadRoot = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

        public UploadController(IUploadService uploadService)
        {
            _uploadService = uploadService;
        }

        [Authorize(Roles = "Instructor")]
        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm] UploadFileDto dto)
        {
            try
            {
               
                if (!Directory.Exists(_uploadRoot))
                    Directory.CreateDirectory(_uploadRoot);

                
                var sanitizedFileName = Path.GetFileName(dto.File.FileName);

              
                var filePath = Path.Combine(_uploadRoot, sanitizedFileName);

            
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.File.CopyToAsync(stream);
                }

                
                var fileMeta = await _uploadService.UploadFileAsync(new UploadedFileDto
                {
                    FileName = sanitizedFileName,
                    Path = filePath,
                    UploadedBy = User.Identity?.Name ?? "Unknown"
                });

                return Ok(new { success = true, data = fileMeta });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "File upload failed.",
                    error = ex.Message
                });
            }
        }

        [HttpGet("{filename}")]
        public IActionResult Download(string filename)
        {
            var sanitizedFileName = Path.GetFileName(filename);
            var filePath = Path.Combine(_uploadRoot, sanitizedFileName);

            if (!System.IO.File.Exists(filePath))
                return NotFound(new { success = false, message = "File not found" });

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, "application/octet-stream", sanitizedFileName);
        }
    }
}
