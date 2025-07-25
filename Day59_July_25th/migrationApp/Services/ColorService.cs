using ChienVHShopOnline.Dtos;
using ChienVHShopOnline.Models;
using ChienVHShopOnline.Repositories;

namespace ChienVHShopOnline.Services
{
    public class ColorService : IColorService
    {
        private readonly IColorRepository _repo;

        public ColorService(IColorRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<ColorDto>> GetAllAsync()
        {
            var colors = await _repo.GetAllAsync();
            return colors.Select(c => new ColorDto
            {
                ColorId = c.ColorId,
                ColorName = c.Color1
            });
        }

        public async Task<ColorDto?> GetByIdAsync(int id)
        {
            var color = await _repo.GetByIdAsync(id);
            if (color == null) return null;

            return new ColorDto
            {
                ColorId = color.ColorId,
                ColorName = color.Color1
            };
        }

        public async Task<ColorDto> CreateAsync(CreateColorDto dto)
        {
            var color = new Color
            {
                Color1 = dto.ColorName
            };

            await _repo.AddAsync(color);
            await _repo.SaveChangesAsync();

            return new ColorDto
            {
                ColorId = color.ColorId,
                ColorName = color.Color1
            };
        }

        public async Task<ColorDto?> UpdateAsync(UpdateColorDto dto)
        {
            var color = await _repo.GetByIdAsync(dto.ColorId);
            if (color == null) return null;

            color.Color1 = dto.ColorName;
            _repo.Update(color);
            await _repo.SaveChangesAsync();

            return new ColorDto
            {
                ColorId = color.ColorId,
                ColorName = color.Color1
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var color = await _repo.GetByIdAsync(id);
            if (color == null) return false;

            _repo.Delete(color);
            await _repo.SaveChangesAsync();
            return true;
        }
    }
}
