using System.ComponentModel.DataAnnotations;

namespace UniqIo.ViewModel.Products;

public class PUpdateVM
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal CostPrice { get; set; }
    public decimal SellPrice { get; set; }
    [Range(0, int.MaxValue)]
    public int PCount { get; set; }
    [Range(0, 100)]
    public int Discount { get; set; }
    public int CompanyId { get; set; }
    public string? FileUrl { get; set; }
    public IFormFile File { get; set; }
    public IEnumerable<string>? OtherFilesUrls { get; set; }
    public ICollection<IFormFile>? OtherFiles { get; set; }
}
