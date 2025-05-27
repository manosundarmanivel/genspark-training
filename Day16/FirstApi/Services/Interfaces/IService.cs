namespace FirstApi.Services.Interfaces
{
    public interface IService<T>
    {
        List<T> GetAll();
        T? GetById(int id);
        void Add(T entity);
        void Delete(int id);
    }
}