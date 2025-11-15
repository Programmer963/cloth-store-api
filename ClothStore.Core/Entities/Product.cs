using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClothStore.Core.Entities
{
    public class Product : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public int StockQuantity { get; set; }
        public string? SKU { get; set; }
        public Guid CategoryId { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsFeatured { get; set; } = false;

        [ForeignKey("CategoryId")]
        public Category Category { get; set; } = null!;

        [InverseProperty("Product")]
        public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();

        [InverseProperty("Product")]
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}


