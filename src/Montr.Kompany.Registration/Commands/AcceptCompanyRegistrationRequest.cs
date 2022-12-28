using System;
using MediatR;
using Montr.Core.Models;

namespace Montr.Kompany.Registration.Commands
{
	public class AcceptCompanyRegistrationRequest : IRequest<ApiResult>
	{
		public Guid? UserUid { get; set; }

		public Guid? DocumentUid { get; set; }

		public string StatusCode { get; set; }
	}
}
