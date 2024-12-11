using System.ComponentModel.DataAnnotations;

namespace UniqIo.ViewModel.Auths
{
	public class ForgotPasswordVM
	{
		[Required,EmailAddress]
		public string Email {  get; set; }
	}
}
