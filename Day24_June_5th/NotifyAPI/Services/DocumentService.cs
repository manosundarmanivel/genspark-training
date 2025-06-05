using NotifyAPI.Interfaces;
using NotifyAPI.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using NotifyAPI.Hubs;
using Microsoft.AspNetCore.Http;

namespace NotifyAPI.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<NotificationHub> _hubContext;

        public DocumentService(AppDbContext context, IHubContext<NotificationHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        public async Task UploadDocumentAsync(IFormFile file, string title)
{
    if (file == null || file.Length == 0)
        throw new ArgumentException("File is empty or null.");

    var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
    Directory.CreateDirectory(folderPath);

    var uniqueName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
    var filePath = Path.Combine(folderPath, uniqueName);

    using (var stream = new FileStream(filePath, FileMode.Create))
    {
        await file.CopyToAsync(stream);
    }

    var document = new Document
    {
        Title = title,
        FilePath = filePath,
        UploadedAt = DateTime.UtcNow
    };

    try
    {
        _context.Documents.Add(document);
        await _context.SaveChangesAsync();

        await _hubContext.Clients.All.SendAsync("DocumentUploaded", new
        {
            Title = document.Title,
            UploadedAt = document.UploadedAt
        });
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Upload failed: {ex.Message}");
        throw;
    }
}


        public async Task<IEnumerable<Document>> GetAllDocumentsAsync()
        {
            return await _context.Documents.ToListAsync();
        }
    }
}
