using System.ComponentModel.DataAnnotations;

namespace UniqIo.ViewModel.Auths
{
	public class ResetPasswordVM
	{
		[Required]
		[DataType(DataType.Password)]
		public string NewPassword { get; set; }
		[DataType(DataType.Password), Compare(nameof(NewPassword))]
		public string ConfirmPassword { get; set; }
		public string userid { get; set; }
		public string token { get; set; }
	}
}
