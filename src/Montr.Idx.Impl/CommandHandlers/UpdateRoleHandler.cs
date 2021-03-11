using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Idx.Commands;
using Montr.Idx.Services;

namespace Montr.Idx.Impl.CommandHandlers
{
	public class UpdateRoleHandler : IRequestHandler<UpdateRole, ApiResult>
	{
		private readonly IUnitOfWorkFactory _unitOfWorkFactory;
		private readonly IRoleManager _roleManager;

		public UpdateRoleHandler(IUnitOfWorkFactory unitOfWorkFactory, IRoleManager roleManager)
		{
			_unitOfWorkFactory = unitOfWorkFactory;
			_roleManager = roleManager;
		}

		public async Task<ApiResult> Handle(UpdateRole request, CancellationToken cancellationToken)
		{
			var role = request?.Item ?? throw new ArgumentNullException(nameof(request));

			using (var scope = _unitOfWorkFactory.Create())
			{
				var result = await _roleManager.Update(role, cancellationToken);

				if (result.Success) scope.Commit();

				return result;
			}
		}
	}
}
