using System.ComponentModel.DataAnnotations;

namespace Montr.Idx.Models
{
	public class ExternalRegisterModel
	{
		public string Provider { get; set; }

		public string ReturnUrl { get; set; }

		[Required]
		[EmailAddress]
		[Display(Name = "Email")]
		[StringLength(128)]
		public string Email { get; set; }

		[StringLength(128)]
		public string FirstName { get; set; }

		[StringLength(128)]
		public string LastName { get; set; }
	}
}
