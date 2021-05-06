using System;
using System.Collections.Generic;
using MediatR;
using Montr.Core.Models;

namespace Montr.Idx.Commands
{
	public class ManageRolePermissions : IRequest<ApiResult>
	{
		public Guid RoleUid { get; set; }

		public IList<string> AddPermissions { get; set; }

		public IList<string> RemovePermissions { get; set; }
	}
}
