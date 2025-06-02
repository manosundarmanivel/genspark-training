using BankingApp.Models;

namespace BankingApp.Auth
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(User user);
    }

    public class DummyJwtTokenGenerator : IJwtTokenGenerator
    {
        public string GenerateToken(User user)
        {
            return $"dummy-token-for-{user.Username}-{user.Id}";
        }
    }
}
