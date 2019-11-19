using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
using Montr.Idx.Commands;
using Montr.Idx.Impl.Entities;

namespace Montr.Idx.Impl.CommandHandlers
{
	public class UpdateProfileHandler : IRequestHandler<UpdateProfile, ApiResult>
	{
		private readonly IUnitOfWorkFactory _unitOfWorkFactory;
		private readonly IDbContextFactory _dbContextFactory;
		private readonly UserManager<DbUser> _userManager;

		public UpdateProfileHandler(
			IUnitOfWorkFactory unitOfWorkFactory,
			IDbContextFactory dbContextFactory,
			UserManager<DbUser> userManager)
		{
			_unitOfWorkFactory = unitOfWorkFactory;
			_dbContextFactory = dbContextFactory;
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
				int affected;

				using (var db = _dbContextFactory.Create())
				{
					affected = await db.GetTable<DbUser>()
						.Where(x => x.Id == user.Id)
						.Set(x => x.LastName, request.LastName)
						.Set(x => x.FirstName, request.FirstName)
						.UpdateAsync(cancellationToken);
				}

				// todo: events

				scope.Commit();

				return new ApiResult { AffectedRows = affected };
			}
		}
	}
}
