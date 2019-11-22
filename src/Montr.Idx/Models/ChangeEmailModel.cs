using System.ComponentModel.DataAnnotations;

namespace Montr.Idx.Models
{
	public class ChangeEmailModel
	{
		[Required]
		[EmailAddress]
		[Display(Name = "Email")]
		[StringLength(128)]
		public string Email { get; set; }
	}
}
