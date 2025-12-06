using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ClothStore.Application.Services;
using ClothStore.Core.Entities;
using ClothStore.API.Controllers.Dtos;

namespace ClothStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IProductImageService _productImageService;

        public ProductController(IProductService productService, IProductImageService productImageService)
        {
            _productService = productService;
            _productImageService = productImageService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Product>>> GetAll()
        {
            var products = await _productService.GetAllAsync();
            return Ok(products);
        }

        [HttpGet("active")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Product>>> GetActive()
        {
            var products = await _productService.GetActiveProductsAsync();
            return Ok(products);
        }

        [HttpGet("featured")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Product>>> GetFeatured()
        {
            var products = await _productService.GetFeaturedProductsAsync();
            return Ok(products);
        }

        [HttpGet("category/{categoryId}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Product>>> GetByCategory(Guid categoryId)
        {
            var products = await _productService.GetProductsByCategoryAsync(categoryId);
            return Ok(products);
        }

        [HttpGet("search")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Product>>> Search([FromQuery] string term)
        {
            if (string.IsNullOrWhiteSpace(term))
            {
                return BadRequest("Search term is required");
            }

            var products = await _productService.SearchProductsAsync(term);
            return Ok(products);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<Product>> GetById(Guid id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "ADMIN")]
        public async Task<ActionResult<Product>> Create([FromBody] CreateProductDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                DiscountPrice = dto.DiscountPrice,
                StockQuantity = dto.StockQuantity,
                SKU = dto.SKU,
                CategoryId = dto.CategoryId,
                IsActive = dto.IsActive,
                IsFeatured = dto.IsFeatured
            };

            var created = await _productService.CreateAsync(product);
            // Загружаем продукт с связями для возврата
            var productWithRelations = await _productService.GetByIdAsync(created.Id);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, productWithRelations);
        }

        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "ADMIN")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != dto.Id)
            {
                return BadRequest("Product ID mismatch");
            }

            var existingProduct = await _productService.GetByIdAsync(id);
            if (existingProduct == null)
            {
                return NotFound();
            }

            existingProduct.Name = dto.Name;
            existingProduct.Description = dto.Description;
            existingProduct.Price = dto.Price;
            existingProduct.DiscountPrice = dto.DiscountPrice;
            existingProduct.StockQuantity = dto.StockQuantity;
            existingProduct.SKU = dto.SKU;
            existingProduct.CategoryId = dto.CategoryId;
            existingProduct.IsActive = dto.IsActive;
            existingProduct.IsFeatured = dto.IsFeatured;

            var updated = await _productService.UpdateAsync(existingProduct);
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "ADMIN")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _productService.DeleteAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        // Product Images endpoints
        [HttpGet("{productId}/images")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ProductImage>>> GetProductImages(Guid productId)
        {
            var images = await _productImageService.GetByProductIdAsync(productId);
            return Ok(images);
        }

        [HttpPost("{productId}/images")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "ADMIN")]
        public async Task<ActionResult<ProductImage>> AddProductImage(Guid productId, [FromBody] AddProductImageDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Проверяем, существует ли продукт
            var product = await _productService.GetByIdAsync(productId);
            if (product == null)
            {
                return NotFound("Product not found");
            }

            var productImage = await _productImageService.CreateAsync(
                productId, 
                dto.UploadId, 
                dto.Order ?? 0, 
                dto.IsPrimary ?? false
            );

            return CreatedAtAction(nameof(GetProductImages), new { productId }, productImage);
        }

        [HttpDelete("{productId}/images/{imageId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "ADMIN")]
        public async Task<IActionResult> DeleteProductImage(Guid productId, Guid imageId)
        {
            var result = await _productImageService.DeleteAsync(imageId);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpPut("{productId}/images/{imageId}/primary")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "ADMIN")]
        public async Task<IActionResult> SetPrimaryImage(Guid productId, Guid imageId)
        {
            var result = await _productImageService.SetPrimaryAsync(imageId);
            if (!result)
            {
                return NotFound();
            }
            return Ok();
        }
    }
}


