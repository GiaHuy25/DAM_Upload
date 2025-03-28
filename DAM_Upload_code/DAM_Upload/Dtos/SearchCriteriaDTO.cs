namespace DAM_Upload.Dtos
{
    public class SearchCriteriaDTO
    {
        public string? Name { get; set; } 
        public string? Type { get; set; } 
        public DateTime? CreatedFrom { get; set; } 
        public DateTime? CreatedTo { get; set; } 
        public string? Format { get; set; }
    }
}
