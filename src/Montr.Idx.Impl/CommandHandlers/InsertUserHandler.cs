using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Core.Models;
using Montr.Idx.Commands;
using Montr.Idx.Services;

namespace Montr.Idx.Impl.CommandHandlers
{
	public class InsertUserHandler : IRequestHandler<InsertUser, ApiResult>
	{
		private readonly IUserManager _userManager;

		public InsertUserHandler(IUserManager userManager)
		{
			_userManager = userManager;
		}

		public async Task<ApiResult> Handle(InsertUser request, CancellationToken cancellationToken)
		{
			var user = request?.Item ?? throw new ArgumentNullException(nameof(request));

			return await _userManager.Create(user, cancellationToken);
		}
	}
}
