using ChienVHShopOnline.Dtos;
using ChienVHShopOnline.Models;
using ChienVHShopOnline.Repositories;

namespace ChienVHShopOnline.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repo;

        public ProductService(IProductRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<ProductDto>> GetAllAsync()
        {
            var products = await _repo.GetAllAsync();
            return products.Select(p => new ProductDto
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                Image = p.Image,
                Price = p.Price,
                UserId = p.UserId,
                CategoryId = p.CategoryId,
                ColorId = p.ColorId,
                ModelId = p.ModelId,
                SellStartDate = p.SellStartDate,
                SellEndDate = p.SellEndDate,
                IsNew = p.IsNew
            });
        }

        public async Task<ProductDto?> GetByIdAsync(int id)
        {
            var p = await _repo.GetByIdAsync(id);
            if (p == null) return null;

            return new ProductDto
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                Image = p.Image,
                Price = p.Price,
                UserId = p.UserId,
                CategoryId = p.CategoryId,
                ColorId = p.ColorId,
                ModelId = p.ModelId,
                SellStartDate = p.SellStartDate,
                SellEndDate = p.SellEndDate,
                IsNew = p.IsNew
            };
        }

        public async Task<ProductDto> CreateAsync(CreateProductDto dto)
        {
            var p = new Product
            {
                ProductName = dto.ProductName,
                Image = dto.Image,
                Price = dto.Price,
                UserId = dto.UserId,
                CategoryId = dto.CategoryId,
                ColorId = dto.ColorId,
                ModelId = dto.ModelId,
                SellStartDate = dto.SellStartDate,
                SellEndDate = dto.SellEndDate,
                IsNew = dto.IsNew
            };
            await _repo.AddAsync(p);
            await _repo.SaveChangesAsync();

            return new ProductDto
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                Image = p.Image,
                Price = p.Price,
                UserId = p.UserId,
                CategoryId = p.CategoryId,
                ColorId = p.ColorId,
                ModelId = p.ModelId,
                SellStartDate = p.SellStartDate,
                SellEndDate = p.SellEndDate,
                IsNew = p.IsNew
            };
        }

        public async Task<ProductDto?> UpdateAsync(UpdateProductDto dto)
        {
            var p = await _repo.GetByIdAsync(dto.ProductId);
            if (p == null) return null;

            p.ProductName = dto.ProductName;
            p.Image = dto.Image;
            p.Price = dto.Price;
            p.UserId = dto.UserId;
            p.CategoryId = dto.CategoryId;
            p.ColorId = dto.ColorId;
            p.ModelId = dto.ModelId;
            p.SellStartDate = dto.SellStartDate;
            p.SellEndDate = dto.SellEndDate;
            p.IsNew = dto.IsNew;

            _repo.Update(p);
            await _repo.SaveChangesAsync();

            return await GetByIdAsync(p.ProductId);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var p = await _repo.GetByIdAsync(id);
            if (p == null) return false;

            _repo.Delete(p);
            await _repo.SaveChangesAsync();
            return true;
        }
    }
}
