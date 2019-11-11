using System.ComponentModel.DataAnnotations;

namespace Montr.Idx.Models
{
	public class ProfileModel
	{
		public string UserName { get; set; }

		public bool HasPassword { get; set; }

		[Phone]
		[StringLength(12)]
		public string PhoneNumber { get; set; }
	}
}
