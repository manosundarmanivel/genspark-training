using BankingApp.Data;
using Microsoft.EntityFrameworkCore;

public class UserProgressService : IUserProgressService
{
    private readonly BankingDbContext _context;

    public UserProgressService(BankingDbContext context)
    {
        _context = context;
    }

    public async Task<List<UserProgress>> GetUserProgressAsync(Guid userId)
    {
        return await _context.UserProgresses
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.Timestamp)
            .ToListAsync();
    }
}