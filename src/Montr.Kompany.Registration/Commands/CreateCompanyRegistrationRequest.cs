using System;
using MediatR;
using Montr.Core.Models;

namespace Montr.Kompany.Registration.Commands
{
	public class CreateCompanyRegistrationRequest : IRequest<ApiResult>
	{
		public Guid UserUid { get; set; }
	}
}
