using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Montr.Metadata;
using Montr.Metadata.Services.Designers;
using Montr.Settings;

namespace Montr.Messages
{
	public class SmtpOptions
	{
		[Required]
		public string From { get; set; }

		[Required]
		public string Host { get; set; }

		[Required, Range(ushort.MinValue, ushort.MaxValue)]
		public int Port { get; set; } = 587;

		[Required, DefaultValue(true)]
		public bool UseSsl { get; set; } = true;

		[Required]
		public string UserName { get; set; }

		[Required]
		[FieldDesigner(typeof(PasswordFieldDesigner))]
		public string Password { get; set; }

		public bool TestMode { get; set; }

		[EmailAddress]
		[StringLength(320)]
		public string TestAddress { get; set; }
	}
}
