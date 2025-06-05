
namespace NotifyAPI.Models
{
    public class Document
{
    public int Id { get; set; }
    
    public string Title { get; set; } = string.Empty;

    public string FilePath { get; set; } = string.Empty;

    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
}

}