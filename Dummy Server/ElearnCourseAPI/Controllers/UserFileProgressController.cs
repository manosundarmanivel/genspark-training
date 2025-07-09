using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ElearnAPI.Interfaces.Services;
using ElearnAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ElearnAPI.Controllers
{
    [ApiController]
    [Route("api/v1/progress")]
    [Authorize(Roles = "Student")]
    public class UserFileProgressController : ControllerBase
    {
        private readonly IUserFileProgressService _progressService;

        public UserFileProgressController(IUserFileProgressService progressService)
        {
            _progressService = progressService;
        }

        private Guid GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return Guid.TryParse(userIdClaim?.Value, out var userId)
                ? userId
                : throw new UnauthorizedAccessException("Invalid user ID.");
        }


        [HttpPost]
        public async Task<IActionResult> AddProgress([FromBody] UserFileProgress progress)
        {
            await _progressService.AddProgressAsync(progress);
            return Ok(new { message = "Progress recorded." });
        }


        [HttpGet("user")]
        public async Task<ActionResult<IEnumerable<UserFileProgress>>> GetByUser()
        {
            var userId = GetUserId();
            var progressList = await _progressService.GetByUserAsync(userId);
            return Ok(progressList);
        }

        [HttpGet("file/{fileId}")]
        public async Task<ActionResult<UserFileProgress?>> GetByUserAndFile(Guid fileId)
        {
            var userId = GetUserId();
            var progress = await _progressService.GetByUserAndFileAsync(userId, fileId);
            if (progress == null)
                return NotFound();
            return Ok(progress);
        }


        [HttpPost("complete/{fileId}")]

        public async Task<IActionResult> MarkAsCompleted(Guid fileId)
        {
            var userId = GetUserId();
            Console.WriteLine($"[LOG] Hitting MarkAsCompleted. UserId: {userId}, FileId: {fileId}");

            await _progressService.MarkAsCompletedAsync(userId, fileId);
            return Ok(new { message = "Marked as completed." });
        }


        [HttpGet("completed-files/{courseId}")]
        public async Task<ActionResult<List<Guid>>> GetCompletedFileIds(Guid courseId)
        {
            var userId = GetUserId();
            var completedIds = await _progressService.GetCompletedFileIdsAsync(userId, courseId);
            return Ok(completedIds);
        }
    }
}
