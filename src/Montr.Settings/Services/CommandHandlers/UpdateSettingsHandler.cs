using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Settings.Commands;

namespace Montr.Settings.Services.CommandHandlers
{
	public class UpdateSettingsHandler : IRequestHandler<UpdateSettings, ApiResult>
	{
		private readonly IUnitOfWorkFactory _unitOfWorkFactory;
		private readonly ISettingsTypeRegistry _settingsTypeRegistry;
		private readonly ISettingsRepository _settingsRepository;

		public UpdateSettingsHandler(IUnitOfWorkFactory unitOfWorkFactory,
			ISettingsTypeRegistry settingsTypeRegistry, ISettingsRepository settingsRepository)
		{
			_unitOfWorkFactory = unitOfWorkFactory;
			_settingsTypeRegistry = settingsTypeRegistry;
			_settingsRepository = settingsRepository;
		}

		public async Task<ApiResult> Handle(UpdateSettings request, CancellationToken cancellationToken)
		{
			ApiResult result;

			if (request.Values != null && _settingsTypeRegistry.TryGetType(request.SettingsTypeCode, out var type))
			{
				using (var scope = _unitOfWorkFactory.Create())
				{
					var settings = _settingsRepository.GetSettings(request.EntityTypeCode, request.EntityUid, type);

					foreach (var property in type.GetProperties())
					{
						settings.Set(property.Name, property.GetValue(request.Values));
					}

					var affected = await settings.Update(cancellationToken);

					scope.Commit();

					result = new ApiResult { AffectedRows = affected };
				}
			}
			else
			{
				result = new ApiResult { Success = false };
			}

			return result;
		}
	}
}
