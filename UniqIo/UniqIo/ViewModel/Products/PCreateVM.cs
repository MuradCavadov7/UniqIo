using UniqIo.Models;
using System.ComponentModel.DataAnnotations;

namespace UniqIo.ViewModel.Products;

public class PCreateVM
{

    public string Name { get; set; }
    public string Description { get; set; }
    public decimal CostPrice { get; set; }
    public decimal SellPrice { get; set; }
    [Range(0, int.MaxValue)]
    public int PCount { get; set; }
    [Range(0, 100)]
    public int Discount { get; set; }
    public int CompanyId { get; set; }
    public IFormFile File { get; set; }
    public ICollection<IFormFile>? OtherFiles { get; set; }

    public static implicit operator Product(PCreateVM vm)
    {
        return new Product
        {
            CompanyId = vm.CompanyId,
            CostPrice = vm.CostPrice,
            Description = vm.Description,
            Discount = vm.Discount,
            Name = vm.Name,
            PCount = vm.PCount,
            SellPrice = vm.SellPrice
        };
    }
}


