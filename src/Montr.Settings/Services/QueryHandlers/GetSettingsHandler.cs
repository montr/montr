using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Configuration;
using Montr.Core.Models;
using Montr.Settings.Queries;

namespace Montr.Settings.Services.QueryHandlers
{
	public class GetSettingsHandler : IRequestHandler<GetSettings, ApiResult<object>>
	{
		private readonly ISettingsTypeRegistry _settingsTypeRegistry;
		private readonly IConfiguration _configuration;

		public GetSettingsHandler(ISettingsTypeRegistry settingsTypeRegistry, IConfiguration configuration)
		{
			_settingsTypeRegistry = settingsTypeRegistry;
			_configuration = configuration;
		}

		public Task<ApiResult<object>> Handle(GetSettings request, CancellationToken cancellationToken)
		{
			ApiResult<object> result;

			if (_settingsTypeRegistry.TryGetType(request.SettingsTypeCode, out var type))
			{
				var typeCode = _settingsTypeRegistry.GetTypeCode(type);

				var options = _configuration.GetSection(typeCode).Get(type);

				result = new ApiResult<object> { Data = options };
			}
			else
			{
				result =  new ApiResult<object> { Success = false };
			}

			return Task.FromResult(result);
		}
	}
}
