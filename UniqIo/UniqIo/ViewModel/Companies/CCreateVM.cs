using System.ComponentModel.DataAnnotations;

namespace UniqIo.ViewModel.Companies
{
    public class CCreateVM
    {
        [MaxLength(32, ErrorMessage = "Title can accept max 32 characters.")]
        [Required]
        public string Name { get; set; }
    }
}
