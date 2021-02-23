using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Core.Models;
using Montr.Idx.Commands;
using Montr.Idx.Services;

namespace Montr.Idx.Impl.CommandHandlers
{
	public class UpdateUserHandler : IRequestHandler<UpdateUser, ApiResult>
	{
		private readonly IUserManager _userManager;

		public UpdateUserHandler(IUserManager userManager)
		{
			_userManager = userManager;
		}

		public async Task<ApiResult> Handle(UpdateUser request, CancellationToken cancellationToken)
		{
			var user = request?.Item ?? throw new ArgumentNullException(nameof(request));

			return await _userManager.Update(user, cancellationToken);
		}
	}
}
