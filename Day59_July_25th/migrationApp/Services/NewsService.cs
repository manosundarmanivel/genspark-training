using ChienVHShopOnline.Dtos;
using ChienVHShopOnline.Models;
using ChienVHShopOnline.Repositories;

namespace ChienVHShopOnline.Services
{
    public class NewsService : INewsService
    {
        private readonly INewsRepository _repo;

        public NewsService(INewsRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<NewsDto>> GetAllAsync()
        {
            var newsList = await _repo.GetAllAsync();
            return newsList.Select(n => new NewsDto
            {
                NewsId = n.NewsId,
                UserId = n.UserId,
                Title = n.Title,
                ShortDescription = n.ShortDescription,
                Image = n.Image,
                Content = n.Content,
                CreatedDate = n.CreatedDate,
                Status = n.Status
            });
        }

        public async Task<NewsDto?> GetByIdAsync(int id)
        {
            var news = await _repo.GetByIdAsync(id);
            if (news == null) return null;

            return new NewsDto
            {
                NewsId = news.NewsId,
                UserId = news.UserId,
                Title = news.Title,
                ShortDescription = news.ShortDescription,
                Image = news.Image,
                Content = news.Content,
                CreatedDate = news.CreatedDate,
                Status = news.Status
            };
        }

        public async Task<NewsDto> CreateAsync(CreateNewsDto dto)
        {
            var news = new News
            {
                UserId = dto.UserId,
                Title = dto.Title,
                ShortDescription = dto.ShortDescription,
                Image = dto.Image,
                Content = dto.Content,
                CreatedDate = dto.CreatedDate,
                Status = dto.Status
            };

            await _repo.AddAsync(news);
            await _repo.SaveChangesAsync();

            return await GetByIdAsync(news.NewsId) ?? throw new Exception("News creation failed");
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var news = await _repo.GetByIdAsync(id);
            if (news == null) return false;

            _repo.Delete(news);
            await _repo.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateAsync(int id, CreateNewsDto dto)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return false;

            existing.UserId = dto.UserId;
            existing.Title = dto.Title;
            existing.ShortDescription = dto.ShortDescription;
            existing.Image = dto.Image;
            existing.Content = dto.Content;
            existing.Status = dto.Status;
            existing.CreatedDate = dto.CreatedDate ?? existing.CreatedDate;

            _repo.Update(existing);
            await _repo.SaveChangesAsync();

            return true;
        }
    }
}
