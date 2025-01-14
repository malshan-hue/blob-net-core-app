using System.ComponentModel.DataAnnotations;

namespace blob_net_core_app.Models
{
    public class FileUploadViewModel
    {
        [Required]
        [Display(Name = "Files")]
        public List<IFormFile> Files { get; set; }
    }
}
