using UniqIo.Models;
using UniqIo.ViewModel.Companies;
using UniqIo.ViewModel.Products;
using UniqIo.ViewModel.Sliders;

namespace UniqIo.ViewModel.Commons;

public class HomeVM
{
	public IEnumerable<SListItemVM> Sliders {  get; set; }
	public IEnumerable<PListItemVM> PopularProducts { get; set; }
	public IEnumerable<Company> Companies { get; set; }
}
