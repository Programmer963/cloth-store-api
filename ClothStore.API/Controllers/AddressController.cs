using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ClothStore.Application.Services;
using ClothStore.Core.Entities;

namespace ClothStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AddressController : ControllerBase
    {
        private readonly IAddressService _addressService;

        public AddressController(IAddressService addressService)
        {
            _addressService = addressService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Address>>> GetMyAddresses()
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
            var addresses = await _addressService.GetAddressesByUserIdAsync(userId);
            return Ok(addresses);
        }

        [HttpGet("default")]
        public async Task<ActionResult<Address>> GetDefault()
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
            var address = await _addressService.GetDefaultAddressAsync(userId);
            if (address == null)
            {
                return NotFound();
            }
            return Ok(address);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Address>> GetById(Guid id)
        {
            var address = await _addressService.GetByIdAsync(id);
            if (address == null)
            {
                return NotFound();
            }

            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
            if (address.UserId != userId)
            {
                return Forbid();
            }

            return Ok(address);
        }

        [HttpPost]
        public async Task<ActionResult<Address>> Create([FromBody] Address address)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
            address.UserId = userId;

            var created = await _addressService.CreateAsync(address);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] Address address)
        {
            if (id != address.Id)
            {
                return BadRequest();
            }

            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
            var existing = await _addressService.GetByIdAsync(id);
            if (existing == null || existing.UserId != userId)
            {
                return Forbid();
            }

            address.UserId = userId;
            var updated = await _addressService.UpdateAsync(address);
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
            var address = await _addressService.GetByIdAsync(id);
            if (address == null || address.UserId != userId)
            {
                return Forbid();
            }

            var result = await _addressService.DeleteAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpPut("{id}/set-default")]
        public async Task<IActionResult> SetDefault(Guid id)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
            var address = await _addressService.GetByIdAsync(id);
            if (address == null || address.UserId != userId)
            {
                return Forbid();
            }

            var updated = await _addressService.SetDefaultAddressAsync(id, userId);
            return Ok(updated);
        }
    }
}


