using UniqIo.ViewModel.Products;
using UniqIo.ViewModel.Sliders;

namespace UniqIo.ViewModel.Commons;

public class HomeVM
{
	public IEnumerable<SListItemVM> Sliders {  get; set; }
	public IEnumerable<PListItemVM> Products { get; set; }
}
