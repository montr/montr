using System;
using MediatR;
using Montr.Kompany.Models;

namespace Montr.Kompany.Commands
{
	public class CreateCompany : IRequest<Guid>
	{
		public Guid UserUid { get; set; }

		public Company Company { get; set; }
	}
}
