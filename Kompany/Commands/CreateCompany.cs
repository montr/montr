using System;
using Kompany.Models;
using MediatR;

namespace Kompany.Commands
{
	public class CreateCompany : IRequest<Guid>
	{
		public Guid UserUid { get; set; }

		public Company Company { get; set; }
	}
}
