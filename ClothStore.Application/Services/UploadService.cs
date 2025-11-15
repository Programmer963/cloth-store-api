using ClothStore.Core;
using ClothStore.Core.Entities;

namespace ClothStore.Application.Services
{
    public interface IUploadService : IBaseService<Upload>
    {
    }

    public class UploadService : BaseService<Upload>, IUploadService
    {
        public UploadService(ApplicationDbContext context) : base(context)
        {
        }
    }
}


