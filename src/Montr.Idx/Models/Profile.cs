using System.ComponentModel.DataAnnotations;

namespace Montr.Idx.Models
{
	public class Profile
	{
		public string UserName { get; set; }

		[Phone]
		[StringLength(12)]
		public string PhoneNumber { get; set; }
	}
}
