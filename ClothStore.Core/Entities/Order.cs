using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClothStore.Core.Entities
{
    public class Order : BaseEntity
    {
        public Guid UserId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public decimal SubTotal { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal Total { get; set; }
        public string Status { get; set; } = "Pending"; // Pending, Processing, Shipped, Delivered, Cancelled
        public string? Notes { get; set; }
        public DateTime? ShippedDate { get; set; }
        public DateTime? DeliveredDate { get; set; }

        [ForeignKey("UserId")]
        public AspNetUser User { get; set; } = null!;

        [InverseProperty("Order")]
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}


