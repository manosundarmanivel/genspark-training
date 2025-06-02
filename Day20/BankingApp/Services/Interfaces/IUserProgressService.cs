public interface IUserProgressService
{
    Task<List<UserProgress>> GetUserProgressAsync(Guid userId);
}