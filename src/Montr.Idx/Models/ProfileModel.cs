namespace Montr.Idx.Models
{
	public class ProfileModel
	{
		public string UserName { get; set; }

		public bool HasPassword { get; set; }

		public bool IsEmailConfirmed { get; set; }

		public bool IsPhoneNumberConfirmed { get; set; }

		public string PhoneNumber { get; set; }
	}
}
