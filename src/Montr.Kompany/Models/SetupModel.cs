using System.ComponentModel.DataAnnotations;

namespace Montr.Kompany.Models
{
	public class SetupModel
	{
		[Required]
		public string CompanyName { get; set; }

		[Required]
		[EmailAddress]
		public string AdminEmail { get; set; }

		[Required]
		[DataType(DataType.Password)]
		public string AdminPassword { get; set; }
	}
}
