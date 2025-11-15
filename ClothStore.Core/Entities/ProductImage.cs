using System.ComponentModel.DataAnnotations.Schema;

namespace ClothStore.Core.Entities
{
    public class ProductImage : BaseEntity
    {
        public Guid ProductId { get; set; }
        public Guid UploadId { get; set; }
        public int Order { get; set; }
        public bool IsPrimary { get; set; } = false;

        [ForeignKey("ProductId")]
        public Product Product { get; set; } = null!;

        [ForeignKey("UploadId")]
        public Upload Upload { get; set; } = null!;
    }
}


