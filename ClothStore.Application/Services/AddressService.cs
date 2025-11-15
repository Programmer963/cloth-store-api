using ClothStore.Core;
using ClothStore.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClothStore.Application.Services
{
    public interface IAddressService : IBaseService<Address>
    {
        Task<IEnumerable<Address>> GetAddressesByUserIdAsync(Guid userId);
        Task<Address?> GetDefaultAddressAsync(Guid userId);
        Task<Address> SetDefaultAddressAsync(Guid addressId, Guid userId);
    }

    public class AddressService : BaseService<Address>, IAddressService
    {
        public AddressService(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Address>> GetAddressesByUserIdAsync(Guid userId)
        {
            return await _dbSet
                .Where(x => !x.IsDeleted && x.UserId == userId)
                .OrderByDescending(x => x.IsDefault)
                .ThenByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<Address?> GetDefaultAddressAsync(Guid userId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.UserId == userId && x.IsDefault);
        }

        public async Task<Address> SetDefaultAddressAsync(Guid addressId, Guid userId)
        {
            // Сбрасываем все адреса пользователя
            var userAddresses = await _dbSet
                .Where(x => !x.IsDeleted && x.UserId == userId)
                .ToListAsync();

            foreach (var address in userAddresses)
            {
                address.IsDefault = false;
                address.UpdatedAt = DateTime.UtcNow;
            }

            // Устанавливаем новый адрес по умолчанию
            var defaultAddress = await GetByIdAsync(addressId);
            if (defaultAddress != null && defaultAddress.UserId == userId)
            {
                defaultAddress.IsDefault = true;
                defaultAddress.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return defaultAddress!;
        }
    }
}


