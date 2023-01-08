using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Montr.Core.Services;
using Montr.MasterData.Models;
using Montr.MasterData.Services;
using Montr.Settings.Services;

namespace Montr.Tasks.Services.Implementations
{
	public class RegisterDefaultNumeratorStartupTask : IStartupTask
	{
		private readonly ILogger<RegisterDefaultNumeratorStartupTask> _logger;
		private readonly IUnitOfWorkFactory _unitOfWorkFactory;
		private readonly INamedServiceFactory<IClassifierRepository> _classifierRepositoryFactory;
		private readonly IOptionsMonitor<TasksOptions> _tasksOptionsMonitor;
		private readonly ISettingsRepository _settingsRepository;

		public RegisterDefaultNumeratorStartupTask(ILogger<RegisterDefaultNumeratorStartupTask> logger,
			IUnitOfWorkFactory unitOfWorkFactory,
			INamedServiceFactory<IClassifierRepository> classifierRepositoryFactory,
			IOptionsMonitor<TasksOptions> tasksOptionsMonitor,
			ISettingsRepository settingsRepository)
		{
			_logger = logger;
			_unitOfWorkFactory = unitOfWorkFactory;
			_classifierRepositoryFactory = classifierRepositoryFactory;
			_tasksOptionsMonitor = tasksOptionsMonitor;
			_settingsRepository = settingsRepository;
		}
		
		public async Task Run(CancellationToken cancellationToken)
		{
			await RegisterDefaultNumerator(cancellationToken);
		}
		
		private async Task RegisterDefaultNumerator(CancellationToken cancellationToken)
		{
			var tasksOptions = _tasksOptionsMonitor.CurrentValue;

			if (tasksOptions.DefaultNumeratorId != null)
			{
				_logger.LogDebug("Default tasks numerator {numeratorId} already saved to settings.", tasksOptions.DefaultNumeratorId);

				return;
			}

			var defaultNumerator = GetDefaultNumerator();

			var numeratorRepository =
				_classifierRepositoryFactory.GetRequiredService(MasterData.ClassifierTypeCode.Numerator);

			var numerators = await numeratorRepository.Search(new NumeratorSearchRequest
			{
				TypeCode = MasterData.ClassifierTypeCode.Numerator,
				Code = defaultNumerator.Code
			}, cancellationToken);

			if (numerators.Rows.Count > 0)
			{
				_logger.LogDebug("Default tasks numerator {numeratorId} already created.", numerators.Rows[0].Uid);

				return;
			}

			using (var scope = _unitOfWorkFactory.Create())
			{
				var numerator = await numeratorRepository.Insert(defaultNumerator, cancellationToken);

				numerator.AssertSuccess(() => "Failed to create default tasks numerator.");

				var result = await _settingsRepository.GetApplicationSettings<TasksOptions>()
					.Set(x => x.DefaultNumeratorId, numerator.Uid)
					.Update(cancellationToken);

				result.AssertSuccess(() => "Failed to save default tasks numerator to settings.");

				scope.Commit();

				_logger.LogDebug("Created default tasks numerator {numeratorId} and saved to settings.", numerator.Uid);
			}
		}

		private static Numerator GetDefaultNumerator()
		{
			return new Numerator
			{
				Code = EntityTypeCode.Task,
				Name = "Default tasks numerator",
				EntityTypeCode = EntityTypeCode.Task,
				IsActive = true,
				IsSystem = true,
				Pattern = "T-{Number}"
			};
		}
	}
}
