using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Montr.Settings;
using Montr.Settings.Services.Designers;

namespace Montr.Messages
{
	public class SmtpOptions
	{
		[Required]
		public string From { get; set; }

		[Required]
		public string Host { get; set; }

		[DefaultValue(587)]
		[Range(ushort.MinValue, ushort.MaxValue)]
		public int Port { get; set; }

		[DefaultValue(true)]
		public bool UseSsl { get; set; } = true;

		[Required]
		public string UserName { get; set; }

		[Required]
		[SettingsDesigner(typeof(PasswordFieldDesigner))]
		public string Password { get; set; }

		public bool TestMode { get; set; }

		public string TestAddress { get; set; }
	}
}
