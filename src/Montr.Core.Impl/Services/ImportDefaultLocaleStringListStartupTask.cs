using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Montr.Core.Impl.CommandHandlers;
using Montr.Core.Impl.Entities;
using Montr.Core.Services;
using Montr.Data.Linq2Db;

namespace Montr.Core.Impl.Services
{
	public class ImportDefaultLocaleStringListStartupTask : IStartupTask
	{
		private readonly ILogger<ImportDefaultLocaleStringListStartupTask> _logger;
		private readonly IUnitOfWorkFactory _unitOfWorkFactory;
		private readonly IDbContextFactory _dbContextFactory;
		private readonly EmbeddedResourceProvider _resourceProvider;
		private readonly LocaleStringSerializer _serializer;
		private readonly ILocaleStringImporter _importer;

		public ImportDefaultLocaleStringListStartupTask(
			ILogger<ImportDefaultLocaleStringListStartupTask> logger,
			IUnitOfWorkFactory unitOfWorkFactory, IDbContextFactory dbContextFactory,
			EmbeddedResourceProvider resourceProvider, LocaleStringSerializer serializer, ILocaleStringImporter importer)
		{
			_logger = logger;
			_unitOfWorkFactory = unitOfWorkFactory;
			_dbContextFactory = dbContextFactory;
			_resourceProvider = resourceProvider;
			_serializer = serializer;
			_importer = importer;
		}

		public async Task Run(CancellationToken cancellationToken)
		{
			if (await _dbContextFactory.HasData<DbLocaleString>(cancellationToken) == false)
			{
				_logger.LogInformation("Importing default resources");

				using (var stream = _resourceProvider.GetEmbeddedResourceStream(typeof(Module), "Resources.locale-strings.json"))
				{
					var list = await _serializer.Deserialize(stream, cancellationToken);

					using (var scope = _unitOfWorkFactory.Create())
					{
						await _importer.Import(list, cancellationToken);

						scope.Commit();
					}
				}
			}
		}
	}
}
