using System.ComponentModel.DataAnnotations;

namespace Montr.Idx.Models
{
	public class ChangeProfileModel
	{
		[StringLength(128)]
		public string FirstName { get; set; }

		[StringLength(128)]
		public string LastName { get; set; }
	}
}
