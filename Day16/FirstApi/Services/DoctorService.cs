using FirstApi.Models;
using FirstApi.Repositories.Interfaces;
using FirstApi.Services.Interfaces;

namespace FirstApi.Services
{
    public class DoctorService : IService<Doctor>
    {
        private readonly IRepository<Doctor> _repo;
        public DoctorService(IRepository<Doctor> repo) => _repo = repo;

        public List<Doctor> GetAll() => _repo.GetAll();
        public Doctor? GetById(int id) => _repo.GetById(id);
        public void Add(Doctor entity) => _repo.Add(entity);
        public void Delete(int id) => _repo.Delete(id);
    }
}