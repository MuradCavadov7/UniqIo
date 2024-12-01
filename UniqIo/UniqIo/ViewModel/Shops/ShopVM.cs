using UniqIo.ViewModel.Companies;
using UniqIo.ViewModel.Products;

namespace UniqIo.ViewModel.Shops;

public class ShopVM
{
    public IEnumerable<CompanyAndProductVM> Companies { get; set; }
    public IEnumerable<PListItemVM> Products { get; set; }
    public int ProductCount {  get; set; }
}
