using ClothStore.Core;
using ClothStore.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClothStore.Application.Services
{
    public interface IProductService : IBaseService<Product>
    {
        Task<IEnumerable<Product>> GetActiveProductsAsync();
        Task<IEnumerable<Product>> GetProductsByCategoryAsync(Guid categoryId);
        Task<IEnumerable<Product>> GetFeaturedProductsAsync();
        Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm);
    }

    public class ProductService : BaseService<Product>, IProductService
    {
        public ProductService(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Product>> GetActiveProductsAsync()
        {
            return await _dbSet
                .Include(p => p.Category)
                .Include(p => p.Images)
                    .ThenInclude(pi => pi.Upload)
                .Where(x => !x.IsDeleted && x.IsActive)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(Guid categoryId)
        {
            return await _dbSet
                .Include(p => p.Category)
                .Include(p => p.Images)
                    .ThenInclude(pi => pi.Upload)
                .Where(x => !x.IsDeleted && x.IsActive && x.CategoryId == categoryId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetFeaturedProductsAsync()
        {
            return await _dbSet
                .Include(p => p.Category)
                .Include(p => p.Images)
                    .ThenInclude(pi => pi.Upload)
                .Where(x => !x.IsDeleted && x.IsActive && x.IsFeatured)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm)
        {
            return await _dbSet
                .Include(p => p.Category)
                .Include(p => p.Images)
                    .ThenInclude(pi => pi.Upload)
                .Where(x => !x.IsDeleted && x.IsActive &&
                    (x.Name.Contains(searchTerm) || 
                     (x.Description != null && x.Description.Contains(searchTerm)) ||
                     (x.SKU != null && x.SKU.Contains(searchTerm))))
                .ToListAsync();
        }
    }
}

