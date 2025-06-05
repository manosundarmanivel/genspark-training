using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DoctorAppointment.Interfaces;
using DoctorAppointment.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;

namespace DoctorAppointment.Services
{
public class TokenService : ITokenService
{
    private readonly SymmetricSecurityKey _securityKey;
    private readonly IRepository<int, Doctor> _doctorRepo;

    public TokenService(IConfiguration configuration, IRepository<int, Doctor> doctorRepo)
    {
        _securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Keys:JwtTokenKey"]));
        _doctorRepo = doctorRepo;
    }

    public async Task<string> GenerateToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Username),
            new Claim(ClaimTypes.Role, user.Role)
        };

        if (user.Role == "Doctor")
        {
            var doctors = await _doctorRepo.GetAll();
            var doctor = doctors.FirstOrDefault(d => d.User != null && d.User.Username == user.Username);

            if (doctor != null)
            {
                claims.Add(new Claim("YearsOfExperience", doctor.YearsOfExperience.ToString()));
            }
        }

        var creds = new SigningCredentials(_securityKey, SecurityAlgorithms.HmacSha256Signature);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddDays(1),
            SigningCredentials = creds
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}


}
