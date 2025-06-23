using System;
using System.Collections.Generic;

namespace ElearnAPI.Models
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Username { get; set; } = null!;

        public string PasswordHash { get; set; } = null!;

        public Role Role { get; set; } = null!;

        public string? RefreshToken { get; set; }

        public DateTime? RefreshTokenExpiryTime { get; set; }

        public bool IsDeleted { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Profile fields
        public string? FullName { get; set; }

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public string? ProfilePictureUrl { get; set; }

        public string? Bio { get; set; }

        // Navigation properties
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        
        public ICollection<UserFileProgress> FileProgresses { get; set; } = new List<UserFileProgress>();

    }
}
