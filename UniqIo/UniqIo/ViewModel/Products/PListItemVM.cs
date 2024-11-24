namespace UniqIo.ViewModel.Products;

public class PListItemVM
{
	public int Id {  get; set; }
	public string Name { get; set; }
	public decimal SellPrice { get; set; }
	public int Discount {  get; set; }
	public bool IsStock {  get; set; }
	public string CoverImage {  get; set; }
}
