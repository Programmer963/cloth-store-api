using ClothStore.Core.Entities;
using Microsoft.Extensions.Options;

namespace ClothStore.API.Services
{
    public class FileUtil
    {
        public static string SaveFileLocally(IFormFile file, IOptions<FileUploadSettings> fileUploadSettings)
        {
            string targetDirectory = fileUploadSettings.Value.TargetDirectory;

            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("Invalid file");
            }

            Directory.CreateDirectory(targetDirectory);

            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            string filePath = Path.Combine(targetDirectory, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            return filePath;
        }

        public static Upload Upload(IFormFile file, IOptions<FileUploadSettings> fileUploadSettings)
        {
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            Upload upload = new Upload();
            upload.CreatedAt = DateTime.UtcNow;
            upload.UpdatedAt = DateTime.UtcNow;
            upload.Name = file.FileName;
            upload.FilePath = SaveFileLocally(file, fileUploadSettings);
            upload.Size = (double)file.Length / 1024 / 1024; // MB
            upload.ContentType = file.ContentType;
            upload.Extension = extension;
            return upload;
        }
    }

    public class FileUploadSettings
    {
        public string TargetDirectory { get; set; } = string.Empty;
    }
}


