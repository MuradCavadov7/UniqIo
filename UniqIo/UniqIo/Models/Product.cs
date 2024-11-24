using System.ComponentModel.DataAnnotations;
using System.Drawing.Drawing2D;

namespace UniqIo.Models;

public class Product :BaseEntity
{

    [MaxLength(64)]
    public string Name { get; set; } = null!;
    [MaxLength(512)]
    public string Description { get; set; }
    public string CoverImage { get; set; } = null!;
    [Range(0, int.MaxValue)]
    public int PCount { get; set; }
    [DataType("decimal(18,2)")]
    public decimal CostPrice { get; set; }
    [DataType("decimal(18,2)")]
    public decimal SellPrice { get; set; }
    [Range(0, 100)]
    public int Discount { get; set; }
    public int? CompanyId { get; set; }
    public Company? Company { get; set; }
    public ICollection<ProductImage> Images { get; set; }
}
