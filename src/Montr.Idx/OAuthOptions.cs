using System.ComponentModel.DataAnnotations;
using Montr.Metadata;
using Montr.Metadata.Services.Designers;

namespace Montr.Idx
{
	public abstract class OAuthOptions
	{
		[Required]
		public string ClientId { get; set; }

		[Required]
		[FieldDesigner(typeof(PasswordFieldDesigner))]
		public string ClientSecret { get; set; }
	}
}
