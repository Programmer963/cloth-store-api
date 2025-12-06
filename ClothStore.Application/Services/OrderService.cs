using ClothStore.Core;
using ClothStore.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClothStore.Application.Services
{
    public interface IOrderService : IBaseService<Order>
    {
        Task<IEnumerable<Order>> GetOrdersByUserIdAsync(Guid userId);
        Task<Order?> GetOrderByOrderNumberAsync(string orderNumber);
        Task<Order> CreateOrderAsync(Order order, List<OrderItem> orderItems);
    }

    public class OrderService : BaseService<Order>, IOrderService
    {
        public OrderService(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(Guid userId)
        {
            return await _dbSet
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Where(x => !x.IsDeleted && x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<Order?> GetOrderByOrderNumberAsync(string orderNumber)
        {
            return await _dbSet
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                        .ThenInclude(p => p.Images)
                            .ThenInclude(pi => pi.Upload)
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.OrderNumber == orderNumber);
        }

        public async Task<Order> CreateOrderAsync(Order order, List<OrderItem> orderItems)
        {
            order.CreatedAt = DateTime.UtcNow;
            order.UpdatedAt = DateTime.UtcNow;
            order.OrderNumber = GenerateOrderNumber();

            await _dbSet.AddAsync(order);
            await _context.SaveChangesAsync();

            foreach (var item in orderItems)
            {
                item.OrderId = order.Id;
                item.CreatedAt = DateTime.UtcNow;
                item.UpdatedAt = DateTime.UtcNow;
                await _context.Set<OrderItem>().AddAsync(item);
            }

            await _context.SaveChangesAsync();
            return order;
        }

        private string GenerateOrderNumber()
        {
            return $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper()}";
        }
    }
}


