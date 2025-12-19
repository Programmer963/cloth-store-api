using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ClothStore.Application.Services;
using ClothStore.Core.Entities;
using ClothStore.API.Controllers.Dtos;

namespace ClothStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetMyOrders()
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
            var orders = await _orderService.GetOrdersByUserIdAsync(userId);
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetById(Guid id)
        {
            var order = await _orderService.GetByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
            var isAdmin = User.IsInRole("ADMIN");

            if (!isAdmin && order.UserId != userId)
            {
                return Forbid();
            }

            return Ok(order);
        }

        [HttpGet("number/{orderNumber}")]
        public async Task<ActionResult<Order>> GetByOrderNumber(string orderNumber)
        {
            var order = await _orderService.GetOrderByOrderNumberAsync(orderNumber);
            if (order == null)
            {
                return NotFound();
            }

            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
            var isAdmin = User.IsInRole("ADMIN");

            if (!isAdmin && order.UserId != userId)
            {
                return Forbid();
            }

            return Ok(order);
        }

        [HttpPost]
        public async Task<ActionResult<Order>> Create([FromBody] CreateOrderDto dto)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());

            // Устанавливаем статус в зависимости от метода оплаты
            // Если оплата картой, статус "Paid", иначе "Pending"
            var status = !string.IsNullOrEmpty(dto.PaymentMethod) && dto.PaymentMethod.ToLower() == "card" 
                ? "Paid" 
                : "Pending";

            var order = new Order
            {
                UserId = userId,
                SubTotal = dto.SubTotal,
                ShippingCost = dto.ShippingCost,
                Total = dto.Total,
                Status = status,
                Notes = dto.Notes
            };

            var orderItems = dto.Items.Select(item => new OrderItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                TotalPrice = item.Quantity * item.UnitPrice
            }).ToList();

            var created = await _orderService.CreateOrderAsync(order, orderItems);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateOrderStatusDto dto)
        {
            var order = await _orderService.GetByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            order.Status = dto.Status;
            if (dto.Status == "Shipped")
            {
                order.ShippedDate = DateTime.UtcNow;
            }
            if (dto.Status == "Delivered")
            {
                order.DeliveredDate = DateTime.UtcNow;
            }

            var updated = await _orderService.UpdateAsync(order);
            return Ok(updated);
        }
    }
}

