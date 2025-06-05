using NotifyAPI.Models;
namespace NotifyAPI.Interfaces
{
   public interface IDocumentService
    {
        Task UploadDocumentAsync(IFormFile file, string title);
        Task<IEnumerable<Document>> GetAllDocumentsAsync(); 
    }
}