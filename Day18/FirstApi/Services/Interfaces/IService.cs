namespace FirstApi.Services.Interfaces
{
    public interface IService<T> where T : class
    {
        Task<List<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        Task AddAsync(T entity);
        Task DeleteAsync(int id);
    }
}
