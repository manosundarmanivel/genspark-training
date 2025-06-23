using System;

namespace ElearnAPI.Models
{
    public class UserFileProgress
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid UserId { get; set; }

        public Guid UploadedFileId { get; set; }

        public bool IsCompleted { get; set; } = false;

        public DateTime? CompletedAt { get; set; }

        public User User { get; set; } = null!;
        public UploadedFile UploadedFile { get; set; } = null!;
    }
}
