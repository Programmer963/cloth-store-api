namespace ClothStore.Core.Entities
{
    public class Upload : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public double Size { get; set; }
        public string FilePath { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public string Extension { get; set; } = string.Empty;
        public Guid? UploaderId { get; set; }
    }
}


