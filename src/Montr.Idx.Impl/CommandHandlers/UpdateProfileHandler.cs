using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Idx.Commands;
using Montr.Idx.Impl.Entities;
using Montr.Idx.Impl.Services;

namespace Montr.Idx.Impl.CommandHandlers
{
	public class UpdateProfileHandler : IRequestHandler<UpdateProfile, ApiResult>
	{
		private readonly IUnitOfWorkFactory _unitOfWorkFactory;
		private readonly UserManager<DbUser> _userManager;

		public UpdateProfileHandler(
			IUnitOfWorkFactory unitOfWorkFactory,
			UserManager<DbUser> userManager)
		{
			_unitOfWorkFactory = unitOfWorkFactory;
			_userManager = userManager;
		}

		public async Task<ApiResult> Handle(UpdateProfile request, CancellationToken cancellationToken)
		{
			var user = await _userManager.GetUserAsync(request.User);
			if (user == null)
			{
				return new ApiResult { Success = false };
				// return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
			}

			using (var scope = _unitOfWorkFactory.Create())
			{
				user.LastName = request.LastName;
				user.FirstName = request.FirstName;

				var result = await _userManager.UpdateAsync(user);

				if (result.Succeeded == false)
				{
					return result.ToApiResult();
				}

				scope.Commit();

				return new ApiResult();
			}
		}
	}
}
