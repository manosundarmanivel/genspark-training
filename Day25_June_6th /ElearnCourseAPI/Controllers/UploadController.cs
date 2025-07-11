using ElearnAPI.DTOs;
using ElearnAPI.Interfaces.Services;
using ElearnAPI.SignalR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Serilog;
using System;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using ElearnAPI.Models;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;


namespace ElearnAPI.Controllers
{
    [ApiController]
    [Route("api/v1/files")]
    public class UploadController : ControllerBase
    {
        private readonly IUploadService _uploadService;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly Serilog.ILogger _logger;
        private readonly string _uploadRoot = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

        private readonly ICourseService _courseService;

        private readonly IUserFileProgressService _userFileProgressService;

        private readonly string _blobContainerSasUrl;


        public UploadController(
              ICourseService courseService,
            IUploadService uploadService,
            IHubContext<NotificationHub> hubContext,
            IUserFileProgressService userFileProgressService,
            IConfiguration configuration)
        {
            _courseService = courseService;
            _uploadService = uploadService;
            _hubContext = hubContext;
            _userFileProgressService = userFileProgressService;
            _blobContainerSasUrl = configuration["AzureStorage:BlobContainerSasUrl"];
            _logger = Log.ForContext<UploadController>();
        }


        [Authorize(Roles = "Instructor")]
        [HttpPost("upload/chunk")]
        public async Task<IActionResult> UploadChunked(
            IFormFile chunk,
            [FromForm] int chunkIndex,
            [FromForm] int totalChunks,
            [FromForm] string fileName,
            [FromForm] Guid courseId,
            [FromForm] string topic,
            [FromForm] string description)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!Guid.TryParse(userIdClaim, out var uploadedBy))
                {
                    _logger.Warning("Invalid token during chunk upload.");
                    return Unauthorized(new { success = false, message = "Invalid token. Cannot extract user ID." });
                }

                // 1. Save chunk to temp folder
                var tempFolder = Path.Combine(Directory.GetCurrentDirectory(), "TempUploads", fileName);
                Directory.CreateDirectory(tempFolder);
                var chunkPath = Path.Combine(tempFolder, $"{chunkIndex}.part");

                using (var stream = new FileStream(chunkPath, FileMode.Create))
                {
                    await chunk.CopyToAsync(stream);
                }

                _logger.Information("Chunk {Index}/{Total} for {File} received.", chunkIndex + 1, totalChunks, fileName);

                // 2. Check if all chunks are received
                var chunkFiles = Directory.GetFiles(tempFolder).Length;
                if (chunkFiles < totalChunks)
                {
                    return Ok(new { success = true, message = "Chunk uploaded, waiting for remaining chunks." });
                }

                // 3. All chunks received — merge
                var uploadsRoot = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
                Directory.CreateDirectory(uploadsRoot);

                var sanitizedFileName = Path.GetFileName(fileName);
                var finalFilePath = Path.Combine(uploadsRoot, sanitizedFileName);

                using (var output = new FileStream(finalFilePath, FileMode.Create))
                {
                    for (int i = 0; i < totalChunks; i++)
                    {
                        var partPath = Path.Combine(tempFolder, $"{i}.part");
                        using var input = new FileStream(partPath, FileMode.Open);
                        await input.CopyToAsync(output);
                    }
                }

                Directory.Delete(tempFolder, true); // cleanup

                var publicUrlPath = $"/uploads/{sanitizedFileName}";

                // 4. Save metadata
                var fileMeta = await _uploadService.UploadFileAsync(new UploadedFileDto
                {
                    FileName = sanitizedFileName,
                    Path = publicUrlPath,
                    CourseId = courseId,
                    Topic = topic,
                    Description = description
                });

                var courseTitle = await _courseService.GetCourseTitleById(courseId);
                await _hubContext.Clients.Group($"course-{courseId}")
               .SendAsync("ReceiveContentUploadNotification", topic, courseTitle, sanitizedFileName);

                _logger.Information("File updated: {FileName} for Course: {CourseId}", sanitizedFileName, courseId);



                return Ok(new
                {
                    success = true,
                    data = new
                    {
                        fileMeta.Id,
                        fileMeta.FileName,
                        fileMeta.Topic,
                        fileMeta.Description,
                        fileMeta.CourseId,
                        path = publicUrlPath
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Chunk upload failed.");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Chunk upload failed.",
                    error = ex.Message
                });
            }
        }



        //azure blob upload

//         [Authorize(Roles = "Instructor")]
// [HttpPost("upload/chunk")]
// public async Task<IActionResult> UploadChunked(
//     IFormFile chunk,
//     [FromForm] int chunkIndex,
//     [FromForm] int totalChunks,
//     [FromForm] string fileName,
//     [FromForm] Guid courseId,
//     [FromForm] string topic,
//     [FromForm] string description)
// {
//     try
//     {
//         var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
//         if (!Guid.TryParse(userIdClaim, out var uploadedBy))
//         {
//             _logger.Warning("Invalid token during chunk upload.");
//             return Unauthorized(new { success = false, message = "Invalid token. Cannot extract user ID." });
//         }

//         // 1. Save current chunk
//         var sanitizedFileName = Path.GetFileName(fileName);
//         var tempFolder = Path.Combine(Directory.GetCurrentDirectory(), "TempUploads", sanitizedFileName);
//         Directory.CreateDirectory(tempFolder);

//         var chunkPath = Path.Combine(tempFolder, $"{chunkIndex}.part");
//         await using (var stream = new FileStream(chunkPath, FileMode.Create))
//         {
//             await chunk.CopyToAsync(stream);
//         }

//         _logger.Information("Chunk {Index}/{Total} for {File} received.", chunkIndex + 1, totalChunks, sanitizedFileName);

//         // 2. Check if all chunks received
//         var chunkFiles = Directory.GetFiles(tempFolder, "*.part").Length;
//         if (chunkFiles < totalChunks)
//         {
//             return Ok(new { success = true, message = "Chunk uploaded, waiting for remaining chunks." });
//         }

//         // 3. Merge chunks into one file
//         var mergedFilePath = Path.Combine(tempFolder, $"_merged_{sanitizedFileName}");
//         await using (var output = new FileStream(mergedFilePath, FileMode.Create))
//         {
//             for (int i = 0; i < totalChunks; i++)
//             {
//                 var partPath = Path.Combine(tempFolder, $"{i}.part");
//                 await using var input = new FileStream(partPath, FileMode.Open);
//                 await input.CopyToAsync(output);
//             }
//         }

//         // 4. Upload to Azure Blob Storage using SAS URL from config
//         var blobBaseUri = new Uri(_blobContainerSasUrl);
//         var containerUri = new Uri($"{blobBaseUri.Scheme}://{blobBaseUri.Host}/uploads/");
//         var blobUri = new Uri(containerUri, sanitizedFileName);

//         var builder = new UriBuilder(blobUri)
//         {
//             Query = blobBaseUri.Query
//         };
//         var blobUriWithSas = builder.Uri;

//         var blobClient = new BlobClient(blobUriWithSas);
//         await using (var mergedStream = new FileStream(mergedFilePath, FileMode.Open))
//         {
//             await blobClient.UploadAsync(mergedStream, overwrite: true);
//         }

//         // 5. Cleanup
//         Directory.Delete(tempFolder, true);

//         // 6. Save metadata
//        var publicUrlPath = blobUriWithSas.ToString(); // Full streaming-capable SAS URL

//         var fileMeta = await _uploadService.UploadFileAsync(new UploadedFileDto
//         {
//             FileName = sanitizedFileName,
//             Path = publicUrlPath,
//             CourseId = courseId,
//             Topic = topic,
//             Description = description
//         });

//         // 7. Notify users
//         var courseTitle = await _courseService.GetCourseTitleById(courseId);
//         await _hubContext.Clients.Group($"course-{courseId}")
//             .SendAsync("ReceiveContentUploadNotification", topic, courseTitle, sanitizedFileName);

//         _logger.Information("File uploaded to Azure: {FileName}", sanitizedFileName);

//         return Ok(new
//         {
//             success = true,
//             data = new
//             {
//                 fileMeta.Id,
//                 fileMeta.FileName,
//                 fileMeta.Topic,
//                 fileMeta.Description,
//                 fileMeta.CourseId,
//                 path = publicUrlPath
//             }
//         });
//     }
//     catch (Exception ex)
//     {
//         _logger.Error(ex, "Chunk upload failed.");
//         return StatusCode(500, new
//         {
//             success = false,
//             message = "Chunk upload failed.",
//             error = ex.Message
//         });
//     }
// }







        [HttpGet("{filename}")]
        public IActionResult Download(string filename)
        {
            var sanitizedFileName = Path.GetFileName(filename);
            var filePath = Path.Combine(_uploadRoot, sanitizedFileName);

            if (!System.IO.File.Exists(filePath))
            {
                _logger.Warning("Download failed. File not found: {FileName}", sanitizedFileName);
                return NotFound(new { success = false, message = "File not found" });
            }

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            _logger.Information("File downloaded: {FileName}", sanitizedFileName);
            return File(fileBytes, "application/octet-stream", sanitizedFileName);
        }

        [Authorize(Roles = "Instructor")]

        [HttpPut("{fileId}")]
        public async Task<IActionResult> Update(Guid fileId, [FromForm] UploadFileDto dto)
        {
            var sanitizedFileName = dto.File?.FileName ?? "existing-file.mp4";
            var newPath = "/uploads/" + sanitizedFileName;

            // Construct DTO with existing fileId
            var updateDto = new UploadedFileDto
            {
                Id = fileId,
                FileName = sanitizedFileName,
                Path = newPath,
                CourseId = dto.CourseId,
                Topic = dto.Topic,
                Description = dto.Description
            };

            var result = await _uploadService.UpdateFileEditAsync(updateDto);
            if (result == null) return NotFound();

            // 5. Send real-time notification
            await _hubContext.Clients.Group($"course-{dto.CourseId}")
                 .SendAsync("ReceiveContentUploadNotification", dto.Topic, sanitizedFileName);

            _logger.Information("File updated: {FileName} for Course: {CourseId}", sanitizedFileName, dto.CourseId);
            return Ok(result);
        }



        [Authorize(Roles = "Instructor")]
        [HttpDelete("{fileId:guid}")]
        public async Task<IActionResult> Delete(Guid fileId)
        {
            try
            {
                var file = await _uploadService.GetFileByIdAsync(fileId);
                if (file == null)
                {
                    _logger.Warning("Delete failed. File not found: {FileId}", fileId);
                    return NotFound(new { success = false, message = "File not found." });
                }

                if (System.IO.File.Exists(file.Path))
                    System.IO.File.Delete(file.Path);

                var success = await _uploadService.DeleteFileAsync(fileId);
                if (!success)
                {
                    _logger.Error("Failed to delete metadata for file: {FileId}", fileId);
                    return StatusCode(500, new { success = false, message = "Failed to delete file metadata." });
                }

                _logger.Information("File deleted: {FileId}", fileId);
                return Ok(new { success = true, message = "File deleted successfully." });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "File deletion failed for FileId: {FileId}", fileId);
                return StatusCode(500, new
                {
                    success = false,
                    message = "File deletion failed.",
                    error = ex.Message
                });
            }
        }
    }
}
