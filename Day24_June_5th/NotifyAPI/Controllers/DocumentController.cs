using NotifyAPI.Models;
using NotifyAPI.Services;
using NotifyAPI.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NotifyAPI.DTOs;


[ApiController]
[Route("api/[controller]")]
public class DocumentController : ControllerBase
{
    private readonly IDocumentService _documentService;

    public DocumentController(IDocumentService documentService) => _documentService = documentService;

[HttpPost("upload")]
[Authorize(Roles = "admin")]
public async Task<IActionResult> Upload([FromForm] FileUploadDto uploadDto)
{
    if (uploadDto.File == null || uploadDto.File.Length == 0)
        return BadRequest("File is empty");

    await _documentService.UploadDocumentAsync(uploadDto.File, uploadDto.Title);
    return Ok("File uploaded successfully");
}



    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAll() => Ok(await _documentService.GetAllDocumentsAsync());
}