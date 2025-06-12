using ElearnAPI.Models;
using System;

namespace ElearnAPI.DTOs
{
    public class UserDtoResponse
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = null!;
      
    }
}