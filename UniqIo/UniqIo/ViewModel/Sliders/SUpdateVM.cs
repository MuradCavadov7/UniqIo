using System.ComponentModel.DataAnnotations;

namespace UniqIo.ViewModel.Sliders;

public class SUpdateVM
{
    [MaxLength(32, ErrorMessage = "Title can accept max 32 characters.")]
    [Required]
    public string Title { get; set; }
    [MaxLength(64, ErrorMessage = "Subtitle can accept max 64 characters.")]
    [Required]
    public string Subtitle { get; set; }
    public string? Link { get; set; }
    [Required(ErrorMessage = "Please select the file.")]
    public string? FileUrl { get; set; }
    public IFormFile File { get; set; }
}
