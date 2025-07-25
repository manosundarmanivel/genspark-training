using ChienVHShopOnline.Data;
using ChienVHShopOnline.Models;

namespace ChienVHShopOnline.Repositories
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(AppDbContext context) : base(context)
        {
        }

        // Add category-specific logic here if needed
    }
}
