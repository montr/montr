using System;
using MediatR;
using Montr.Core.Models;
using Montr.Kompany.Models;

namespace Montr.Kompany.Commands
{
	// todo: remove with old registration ui
	public class CreateCompany : IRequest<ApiResult>
	{
		public abstract class Resources
		{
			public abstract string CompanyCreated { get; }
		}

		public Guid? UserUid { get; set; }

		public Company Item { get; set; }
	}
}
