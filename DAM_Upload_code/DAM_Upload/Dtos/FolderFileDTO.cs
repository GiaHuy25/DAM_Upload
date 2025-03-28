namespace DAM_Upload.Dtos
{
    public class FolderFileDTO
    {
        public int Id { get; set; }
        public string Type { get; set; } // "Folder" hoặc "File"
        public string Name { get; set; }
        public string Path { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int? Size { get; set; } // Chỉ áp dụng cho File
        public string? Format { get; set; }
    }
}
