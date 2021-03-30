using System;
using System.Collections.Generic;
using MediatR;
using Montr.Core.Models;

namespace Montr.Idx.Commands
{
	public class RemoveUserRoles : IRequest<ApiResult>
	{
		public Guid UserUid { get; set; }
		
		public IList<string> Roles { get; set; }
	}
}
