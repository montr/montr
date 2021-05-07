using System;
using System.Collections.Generic;
using MediatR;
using Montr.Core.Models;

namespace Montr.Idx.Commands
{
	public class ManageRolePermissions : IRequest<ApiResult>
	{
		public Guid RoleUid { get; set; }

		public IList<string> Add { get; set; }

		public IList<string> Remove { get; set; }
	}
}
