using System.ComponentModel.DataAnnotations;

namespace ClothStore.API.Controllers.Dtos
{
    public class CreateOrderDto
    {
        [Required]
        public Guid AddressId { get; set; }

        [Required]
        public decimal SubTotal { get; set; }

        [Required]
        public decimal ShippingCost { get; set; }

        [Required]
        public decimal Total { get; set; }

        public string? Notes { get; set; }

        [Required]
        [MinLength(1)]
        public List<OrderItemDto> Items { get; set; } = new List<OrderItemDto>();
    }

    public class OrderItemDto
    {
        [Required]
        public Guid ProductId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal UnitPrice { get; set; }
    }

    public class UpdateOrderStatusDto
    {
        [Required]
        public string Status { get; set; } = string.Empty;
    }
}


