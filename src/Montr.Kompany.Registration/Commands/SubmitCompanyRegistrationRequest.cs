using System;
using MediatR;
using Montr.Core.Models;

namespace Montr.Kompany.Registration.Commands
{
	public class SubmitCompanyRegistrationRequest : IRequest<ApiResult>
	{
		public Guid UserUid { get; set; }

		public Guid DocumentUid { get; set; }
	}
}
