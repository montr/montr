using System.ComponentModel.DataAnnotations;
using MediatR;
using Montr.Core.Models;

namespace Montr.Idx.Commands
{
	public class Login : IRequest<ApiResult>
	{
		public abstract class Resources
		{
			public abstract string Error { get; }

			public abstract string IsLockedOut { get; }

			public abstract string IsNotAllowed { get; }

			public abstract string RequiresTwoFactor { get; }
		}

		public string ReturnUrl { get; set; }

		[Required]
		[EmailAddress]
		[StringLength(128)]
		public string Email { get; set; }

		[Required]
		// [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
		[DataType(DataType.Password)]
		public string Password { get; set; }

		[Display(Name = "Remember me?")]
		public bool RememberMe { get; set; }
	}
}
