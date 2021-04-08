using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
using Montr.MasterData.Impl.Services;
using Montr.MasterData.Models;
using Montr.Metadata.Impl.Services;
using RegisterClassifierTypeStartupTask = Montr.Idx.Impl.Services.RegisterClassifierTypeStartupTask;

namespace Montr.Idx.Tests.Services
{
	public class IdxDbGenerator
	{
		private readonly IDbContextFactory _dbContextFactory;
		private readonly DbClassifierTypeService _classifierTypeService;
		private readonly DefaultClassifierTypeRegistrator _classifierTypeRegistrator;

		public IdxDbGenerator(IUnitOfWorkFactory unitOfWorkFactory, IDbContextFactory dbContextFactory)
		{
			_dbContextFactory = dbContextFactory;

			var classifierTypeRepository = new DbClassifierTypeRepository(dbContextFactory);

			_classifierTypeService = new DbClassifierTypeService(dbContextFactory, classifierTypeRepository);

			_classifierTypeRegistrator = new DefaultClassifierTypeRegistrator(new NullLogger<DefaultClassifierTypeRegistrator>(),
				unitOfWorkFactory, _classifierTypeService, new DbFieldMetadataService(dbContextFactory, new NewtonsoftJsonSerializer()));
		}

		public async Task<ClassifierType> EnsureUserTypeRegistered(CancellationToken cancellationToken)
		{
			var type = RegisterClassifierTypeStartupTask.GetUserType();

			await _classifierTypeRegistrator.Register(type.Item, type.Fields, cancellationToken);

			return await _classifierTypeService.Get(Numerator.TypeCode, cancellationToken);
		}
	}
}
