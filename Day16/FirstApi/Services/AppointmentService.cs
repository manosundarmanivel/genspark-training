using FirstApi.Models;
using FirstApi.Repositories.Interfaces;
using FirstApi.Services.Interfaces;

namespace FirstApi.Services
{
    public class AppointmentService : IService<Appointment>
    {
        private readonly IRepository<Appointment> _repo;
        public AppointmentService(IRepository<Appointment> repo) => _repo = repo;

        public List<Appointment> GetAll() => _repo.GetAll();
        public Appointment? GetById(int id) => _repo.GetById(id);
        public void Add(Appointment entity) => _repo.Add(entity);
        public void Delete(int id) => _repo.Delete(id);
    }
}