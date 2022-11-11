using System.ComponentModel.DataAnnotations;
using Montr.Metadata.Models;
using Montr.Settings;

namespace Montr.Messages
{
	public class SmtpOptions
	{
		[Required]
		public string From { get; set; }

		[Required]
		public string Host { get; set; }

		public int Port { get; set; }

		public bool UseSsl { get; set; } = true;

		[Required]
		public string UserName { get; set; }

		[Required]
		[SettingsDesigner(PasswordField.TypeCode)]
		public string Password { get; set; }

		public bool TestMode { get; set; }

		public string TestAddress { get; set; }
	}
}
