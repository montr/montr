using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
using Montr.Kompany.Commands;

namespace Montr.Kompany.Impl.CommandHandlers
{
	public class SetupApplicationHandler : IRequestHandler<SetupApplication, ApiResult>
	{
		private readonly IUnitOfWorkFactory _unitOfWorkFactory;
		private readonly IDbContextFactory _dbContextFactory;

		public SetupApplicationHandler(IUnitOfWorkFactory unitOfWorkFactory, IDbContextFactory dbContextFactory)
		{
			_unitOfWorkFactory = unitOfWorkFactory;
			_dbContextFactory = dbContextFactory;
		}
		
		public Task<ApiResult> Handle(SetupApplication request, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}
	}
}
