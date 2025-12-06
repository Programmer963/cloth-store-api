using System.ComponentModel.DataAnnotations;

namespace ClothStore.API.Controllers.Dtos
{
    public class CreateProductDto
    {
        [Required(ErrorMessage = "Product name is required")]
        [StringLength(500, ErrorMessage = "Product name cannot exceed 500 characters")]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Discount price must be greater than or equal to 0")]
        public decimal? DiscountPrice { get; set; }

        [Required(ErrorMessage = "Stock quantity is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Stock quantity must be greater than or equal to 0")]
        public int StockQuantity { get; set; }

        public string? SKU { get; set; }

        [Required(ErrorMessage = "Category ID is required")]
        public Guid CategoryId { get; set; }

        public bool IsActive { get; set; } = true;

        public bool IsFeatured { get; set; } = false;
    }

    public class UpdateProductDto
    {
        [Required(ErrorMessage = "Product ID is required")]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Product name is required")]
        [StringLength(500, ErrorMessage = "Product name cannot exceed 500 characters")]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Discount price must be greater than or equal to 0")]
        public decimal? DiscountPrice { get; set; }

        [Required(ErrorMessage = "Stock quantity is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Stock quantity must be greater than or equal to 0")]
        public int StockQuantity { get; set; }

        public string? SKU { get; set; }

        [Required(ErrorMessage = "Category ID is required")]
        public Guid CategoryId { get; set; }

        public bool IsActive { get; set; } = true;

        public bool IsFeatured { get; set; } = false;
    }

    public class AddProductImageDto
    {
        [Required(ErrorMessage = "Upload ID is required")]
        public Guid UploadId { get; set; }

        public int? Order { get; set; }

        public bool? IsPrimary { get; set; }
    }
}

