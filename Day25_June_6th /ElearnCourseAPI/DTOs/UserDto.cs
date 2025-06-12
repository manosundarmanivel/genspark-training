using ElearnAPI.Models;
using System;

namespace ElearnAPI.DTOs
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = null!;
        public Role Role { get; set; } = null!;
    }
}