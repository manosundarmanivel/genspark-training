using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using FirstAPI.Models.DTOs.DoctorSpecialities;

namespace DoctorAppointment.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileController : ControllerBase
    {
        private readonly string _uploadPath;

        public FileController()
        {
            _uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(_uploadPath))
            {
                Directory.CreateDirectory(_uploadPath);
            }
        }

    
       [HttpPost("upload")]
[Consumes("multipart/form-data")]
public async Task<IActionResult> Upload([FromForm] FileUploadDto request)
{
    var file = request.File;

    if (file == null || file.Length == 0)
        return BadRequest("File is missing or empty.");

    var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
    var filePath = Path.Combine(_uploadPath, uniqueFileName);

    using (var stream = new FileStream(filePath, FileMode.Create))
    {
        await file.CopyToAsync(stream);
    }

    return Ok(new
    {
        FileName = uniqueFileName,
        Message = "File uploaded successfully."
    });
}


      
        [HttpGet("download/{fileName}")]
        public async Task<IActionResult> Download(string fileName)
        {
            var filePath = Path.Combine(_uploadPath, fileName);

            if (!System.IO.File.Exists(filePath))
                return NotFound("File not found.");

            var contentType = "application/octet-stream"; 
            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);

            return File(fileBytes, contentType, fileName);
        }
    }
}
