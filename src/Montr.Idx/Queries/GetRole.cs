using System;
using MediatR;
using Montr.Idx.Models;

namespace Montr.Idx.Queries
{
	public class GetRole : RoleSearchRequest, IRequest<Role>
	{
		public Guid Uid { get; set; }
	}
}
