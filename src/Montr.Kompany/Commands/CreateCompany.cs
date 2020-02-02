using System;
using MediatR;
using Montr.Core.Models;
using Montr.Kompany.Models;

namespace Montr.Kompany.Commands
{
	public class CreateCompany : IRequest<ApiResult>
	{
		public Guid UserUid { get; set; }

		public Company Company { get; set; }
	}
}
