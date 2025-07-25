using ChienVHShopOnline.Dtos;
using ChienVHShopOnline.Models;
using ChienVHShopOnline.Repositories;

namespace ChienVHShopOnline.Services
{
    public class ModelService : IModelService
    {
        private readonly IModelRepository _repo;

        public ModelService(IModelRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<ModelDto>> GetAllAsync()
        {
            var models = await _repo.GetAllAsync();
            return models.Select(m => new ModelDto
            {
                ModelId = m.ModelId,
                Model1 = m.Model1
            });
        }

        public async Task<ModelDto?> GetByIdAsync(int id)
        {
            var model = await _repo.GetByIdAsync(id);
            if (model == null) return null;

            return new ModelDto
            {
                ModelId = model.ModelId,
                Model1 = model.Model1
            };
        }

        public async Task<ModelDto> CreateAsync(CreateModelDto dto)
        {
            var model = new Model { Model1 = dto.Model1 };
            await _repo.AddAsync(model);
            await _repo.SaveChangesAsync();

            return new ModelDto
            {
                ModelId = model.ModelId,
                Model1 = model.Model1
            };
        }

        public async Task<ModelDto?> UpdateAsync(UpdateModelDto dto)
        {
            var model = await _repo.GetByIdAsync(dto.ModelId);
            if (model == null) return null;

            model.Model1 = dto.Model1;
            _repo.Update(model);
            await _repo.SaveChangesAsync();

            return new ModelDto
            {
                ModelId = model.ModelId,
                Model1 = model.Model1
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var model = await _repo.GetByIdAsync(id);
            if (model == null) return false;

            _repo.Delete(model);
            await _repo.SaveChangesAsync();
            return true;
        }
    }
}
