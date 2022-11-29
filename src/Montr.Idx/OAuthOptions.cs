using System.ComponentModel.DataAnnotations;
using Montr.Settings;
using Montr.Settings.Services.Designers;

namespace Montr.Idx
{
	public abstract class OAuthOptions
	{
		[Required]
		public string ClientId { get; set; }

		[Required]
		[SettingsDesigner(typeof(PasswordFieldDesigner))]
		public string ClientSecret { get; set; }
	}
}
