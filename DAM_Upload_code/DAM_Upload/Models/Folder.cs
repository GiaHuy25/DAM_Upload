using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace DAM_Upload.Models
{
    public class Folder
    {
        public int FolderId { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int? ParentId { get; set; }
        public string Path { get; set; } = string.Empty;

        public int UserId { get; set; }
        //public StatusFolder status;

        [ForeignKey("ParentId")]

        public virtual Folder ParentFolder { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        public virtual ICollection<Folder> Children { get; set; } = new List<Folder>();

        public virtual ICollection<DAM_Upload.Models.File> Files { get; set; } = new List<DAM_Upload.Models.File>();
    }

    public enum StatusFolder
    {
        PUBLIC, PRIVATE
    }
}
