namespace Montr.Idx.Models
{
	public class ProfileModel
	{
		public string UserName { get; set; }

		public string DisplayName { get; set; }

		public bool HasPassword { get; set; }

		public bool IsEmailConfirmed { get; set; }

		public bool IsPhoneNumberConfirmed { get; set; }

		public string Email { get; set; }

		public string PhoneNumber { get; set; }

		public string FirstName { get; set; }

		public string LastName { get; set; }
	}
}
