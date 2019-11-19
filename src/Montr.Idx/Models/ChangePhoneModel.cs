using System.ComponentModel.DataAnnotations;

namespace Montr.Idx.Models
{
	public class ChangePhoneModel
	{
		[Phone]
		[StringLength(12)]
		public string PhoneNumber { get; set; }
	}
}
