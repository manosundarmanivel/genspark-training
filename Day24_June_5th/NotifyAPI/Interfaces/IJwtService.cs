using NotifyAPI.Models;
namespace NotifyAPI.Interfaces
{

    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}