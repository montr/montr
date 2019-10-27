using System.ComponentModel.DataAnnotations;
using MediatR;
using Montr.Core.Models;

namespace Montr.Idx.Commands
{
	public class RegisterUserCommand : IRequest<ApiResult>
	{
		// todo: remove, use client redirect
		public string ReturnUrl { get; set; }

		[Required]
		[EmailAddress]
		[Display(Name = "Email")]
		[StringLength(128)]
		public string Email { get; set; }

		[StringLength(128)]
		public string FirstName { get; set; }

		[StringLength(128)]
		public string LastName { get; set; }

		[Required]
		[StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
		[DataType(DataType.Password)]
		[Display(Name = "Password")]
		public string Password { get; set; }
	}
}
