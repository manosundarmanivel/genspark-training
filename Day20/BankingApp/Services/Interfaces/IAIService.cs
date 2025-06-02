public interface IAIService
{
    Task<string> GetResponseAsync(string message, List<UserProgress> progress);
}
