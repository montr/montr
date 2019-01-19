using System;
using Kompany.Models;
using MediatR;

namespace Kompany.Requests
{
	public class CreateCompany : IRequest<Guid>
	{
		public Company Company { get; set; }
	}
}
