using ClothStore.Core;
using ClothStore.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClothStore.Application.Services
{
    public interface ICategoryService : IBaseService<Category>
    {
        Task<IEnumerable<Category>> GetActiveCategoriesAsync();
        Task<IEnumerable<Category>> GetRootCategoriesAsync();
        Task<IEnumerable<Category>> GetChildCategoriesAsync(Guid parentId);
    }

    public class CategoryService : BaseService<Category>, ICategoryService
    {
        public CategoryService(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Category>> GetActiveCategoriesAsync()
        {
            return await _dbSet
                .Where(x => !x.IsDeleted && x.IsActive)
                .OrderBy(x => x.Order)
                .ToListAsync();
        }

        public async Task<IEnumerable<Category>> GetRootCategoriesAsync()
        {
            return await _dbSet
                .Where(x => !x.IsDeleted && x.IsActive && x.ParentId == null)
                .OrderBy(x => x.Order)
                .ToListAsync();
        }

        public async Task<IEnumerable<Category>> GetChildCategoriesAsync(Guid parentId)
        {
            return await _dbSet
                .Where(x => !x.IsDeleted && x.IsActive && x.ParentId == parentId)
                .OrderBy(x => x.Order)
                .ToListAsync();
        }
    }
}


