using ElearnAPI.Models;
using ElearnAPI.Services;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ElearnAPI.Tests.Services
{
    public class JwtTokenServiceTests
    {
        private JwtTokenService _tokenService = null!;
        private IConfiguration _configuration = null!;
        private User _testUser = null!;

        [SetUp]
        public void Setup()
        {
            var jwtKey = Convert.ToBase64String(Encoding.UTF8.GetBytes("supersecretkey1234567890")); // 32+ chars
            var inMemorySettings = new Dictionary<string, string>
{
    {"Jwt:Key", "supersecretkey1234567890abcd56789012"}, // 32+ characters
    {"Jwt:Issuer", "TestIssuer"},
    {"Jwt:Audience", "TestAudience"}
};


            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            _tokenService = new JwtTokenService(_configuration);

            _testUser = new User
            {
                Id = Guid.NewGuid(),
                Username = "testuser",
                Role = new Role { Name = "Student" }
            };
        }

        [Test]
        public void GenerateAccessToken_ShouldReturnValidJwt()
        {
            // Act
            var token = _tokenService.GenerateAccessToken(_testUser);

            // Assert: Token should be parsable and have expected claims
            var handler = new JwtSecurityTokenHandler();
            Assert.IsTrue(handler.CanReadToken(token));

            var jwt = handler.ReadJwtToken(token);
            Assert.That(jwt.Claims, Has.Some.Matches<Claim>(c => c.Type == ClaimTypes.Name && c.Value == "testuser"));
            Assert.That(jwt.Claims, Has.Some.Matches<Claim>(c => c.Type == ClaimTypes.Role && c.Value == "Student"));
        }

        [Test]
        public void GenerateRefreshToken_ShouldReturnNonEmptyBase64String()
        {
            var token = _tokenService.GenerateRefreshToken();

            Assert.IsNotNull(token);
            Assert.IsNotEmpty(token);

            // Try decoding to check base64 format
            var decoded = Convert.FromBase64String(token);
            Assert.That(decoded.Length, Is.EqualTo(64));
        }

        [Test]
        public void ValidateToken_ShouldReturnTrueForValidToken()
        {
            var token = _tokenService.GenerateAccessToken(_testUser);

            var isValid = _tokenService.ValidateToken(token);

            Assert.IsTrue(isValid);
        }

        [Test]
        public void ValidateToken_ShouldReturnFalseForInvalidToken()
        {
            var fakeToken = "invalid.token.value";

            var isValid = _tokenService.ValidateToken(fakeToken);

            Assert.IsFalse(isValid);
        }

        [Test]
        public void ValidateToken_ShouldReturnFalseForTamperedToken()
        {
            var token = _tokenService.GenerateAccessToken(_testUser);

            // Tamper the token by replacing some characters
            var tampered = token.Substring(0, token.Length - 2) + "XX";

            var isValid = _tokenService.ValidateToken(tampered);

            Assert.IsFalse(isValid);
        }
    }
}
