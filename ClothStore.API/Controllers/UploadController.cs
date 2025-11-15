using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ClothStore.API.Services;
using ClothStore.Application.Services;
using ClothStore.Core.Entities;
using Microsoft.Extensions.Options;

namespace ClothStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UploadController : ControllerBase
    {
        private readonly IUploadService _uploadService;
        private readonly IOptions<FileUploadSettings> _fileUploadSettings;

        public UploadController(
            IUploadService uploadService,
            IOptions<FileUploadSettings> fileUploadSettings)
        {
            _uploadService = uploadService;
            _fileUploadSettings = fileUploadSettings;
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<Upload>> GetById(Guid id)
        {
            var upload = await _uploadService.GetByIdAsync(id);
            if (upload == null)
            {
                return NotFound();
            }
            return Ok(upload);
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var upload = FileUtil.Upload(file, _fileUploadSettings);
            if (userId != null && Guid.TryParse(userId, out var userIdGuid))
            {
                upload.UploaderId = userIdGuid;
            }

            var createdUpload = await _uploadService.CreateAsync(upload);

            return CreatedAtAction(nameof(GetById), new { id = createdUpload.Id }, createdUpload);
        }

        [HttpGet("{id}/download")]
        [AllowAnonymous]
        public async Task<IActionResult> Download(Guid id)
        {
            var upload = await _uploadService.GetByIdAsync(id);

            if (upload == null)
                return NotFound();

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), upload.FilePath);

            if (!System.IO.File.Exists(filePath))
                return NotFound("File not found on server.");

            var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            return File(stream, upload.ContentType ?? "application/octet-stream", upload.Name);
        }

        [HttpGet("{id}/stream")]
        [AllowAnonymous]
        public async Task<IActionResult> Stream(Guid id)
        {
            var upload = await _uploadService.GetByIdAsync(id);

            if (upload == null)
            {
                return NotFound();
            }

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), upload.FilePath);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            return File(fileStream, upload.ContentType ?? "application/octet-stream");
        }
    }
}


