using ClothStore.Core;
using ClothStore.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClothStore.Application.Services
{
    public interface IProductImageService
    {
        Task<IEnumerable<ProductImage>> GetByProductIdAsync(Guid productId);
        Task<ProductImage> CreateAsync(Guid productId, Guid uploadId, int order = 0, bool isPrimary = false);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> SetPrimaryAsync(Guid id);
        Task<bool> UpdateOrderAsync(Guid id, int order);
    }

    public class ProductImageService : IProductImageService
    {
        private readonly ApplicationDbContext _context;

        public ProductImageService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductImage>> GetByProductIdAsync(Guid productId)
        {
            return await _context.ProductImages
                .Include(pi => pi.Upload)
                .Where(pi => pi.ProductId == productId && !pi.IsDeleted)
                .OrderBy(pi => pi.Order)
                .ToListAsync();
        }

        public async Task<ProductImage> CreateAsync(Guid productId, Guid uploadId, int order = 0, bool isPrimary = false)
        {
            // Проверяем, не существует ли уже это изображение для этого продукта
            var existingImage = await _context.ProductImages
                .FirstOrDefaultAsync(pi => pi.ProductId == productId && pi.UploadId == uploadId && !pi.IsDeleted);

            if (existingImage != null)
            {
                // Если изображение уже существует, возвращаем его
                await _context.Entry(existingImage)
                    .Reference(pi => pi.Upload)
                    .LoadAsync();
                return existingImage;
            }

            // Если это первое изображение или установлено как основное, сбросить другие
            if (isPrimary)
            {
                var existingPrimary = await _context.ProductImages
                    .Where(pi => pi.ProductId == productId && pi.IsPrimary && !pi.IsDeleted)
                    .ToListAsync();

                foreach (var img in existingPrimary)
                {
                    img.IsPrimary = false;
                }
            }

            var productImage = new ProductImage
            {
                ProductId = productId,
                UploadId = uploadId,
                Order = order,
                IsPrimary = isPrimary,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _context.ProductImages.AddAsync(productImage);
            await _context.SaveChangesAsync();

            // Загрузить с Upload для возврата
            await _context.Entry(productImage)
                .Reference(pi => pi.Upload)
                .LoadAsync();

            return productImage;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var productImage = await _context.ProductImages
                .FirstOrDefaultAsync(pi => pi.Id == id && !pi.IsDeleted);

            if (productImage == null)
                return false;

            productImage.IsDeleted = true;
            productImage.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> SetPrimaryAsync(Guid id)
        {
            var productImage = await _context.ProductImages
                .FirstOrDefaultAsync(pi => pi.Id == id && !pi.IsDeleted);

            if (productImage == null)
                return false;

            // Сбросить все остальные основные изображения для этого продукта
            var existingPrimary = await _context.ProductImages
                .Where(pi => pi.ProductId == productImage.ProductId && pi.IsPrimary && !pi.IsDeleted && pi.Id != id)
                .ToListAsync();

            foreach (var img in existingPrimary)
            {
                img.IsPrimary = false;
            }

            productImage.IsPrimary = true;
            productImage.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateOrderAsync(Guid id, int order)
        {
            var productImage = await _context.ProductImages
                .FirstOrDefaultAsync(pi => pi.Id == id && !pi.IsDeleted);

            if (productImage == null)
                return false;

            productImage.Order = order;
            productImage.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return true;
        }
    }
}

