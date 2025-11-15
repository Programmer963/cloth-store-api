using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClothStore.Core.Entities
{
    public class Category : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Guid? ParentId { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; } = true;

        [ForeignKey("ParentId")]
        public Category? Parent { get; set; }

        [InverseProperty("Parent")]
        public ICollection<Category> Children { get; set; } = new List<Category>();

        [InverseProperty("Category")]
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}


