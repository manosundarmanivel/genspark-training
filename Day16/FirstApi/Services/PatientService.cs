using FirstApi.Models;
using FirstApi.Repositories.Interfaces;
using FirstApi.Services.Interfaces;

namespace FirstApi.Services
{
    public class PatientService : IService<Patient>
    {
        private readonly IRepository<Patient> _repo;
        public PatientService(IRepository<Patient> repo) => _repo = repo;

        public List<Patient> GetAll() => _repo.GetAll();
        public Patient? GetById(int id) => _repo.GetById(id);
        public void Add(Patient entity) => _repo.Add(entity);
        public void Delete(int id) => _repo.Delete(id);
    }
}