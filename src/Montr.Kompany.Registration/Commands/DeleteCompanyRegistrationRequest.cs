using System;
using MediatR;
using Montr.Core.Models;

namespace Montr.Kompany.Registration.Commands
{
	public class DeleteCompanyRegistrationRequest : IRequest<ApiResult>
	{
		public Guid UserUid { get; set; }

		public Guid Uid { get; set; }
	}
}
