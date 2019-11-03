using System.ComponentModel.DataAnnotations;

namespace Montr.Idx.Models
{
	public class RegisterModel : ExternalRegisterModel
	{
		[Required]
		[StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
		[DataType(DataType.Password)]
		[Display(Name = "Password")]
		public string Password { get; set; }
	}
}
