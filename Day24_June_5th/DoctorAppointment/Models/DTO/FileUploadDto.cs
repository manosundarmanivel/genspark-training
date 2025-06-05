using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace FirstAPI.Models.DTOs.DoctorSpecialities
{
 public class FileUploadDto
{
    [FromForm]
    public IFormFile File { get; set; }
}

}
