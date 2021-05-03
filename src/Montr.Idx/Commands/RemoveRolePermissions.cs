using System;
using System.Collections.Generic;
using MediatR;
using Montr.Core.Models;

namespace Montr.Idx.Commands
{
	public class RemoveRolePermissions : IRequest<ApiResult>
	{
		public Guid RoleUid { get; set; }

		public IList<string> Permissions { get; set; }
	}
}
