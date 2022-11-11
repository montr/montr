using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Core.Models;
using Montr.Settings.Commands;

namespace Montr.Settings.Services.CommandHandlers
{
	public class UpdateSettingsHandler : IRequestHandler<UpdateSettings, ApiResult>
	{
		public Task<ApiResult> Handle(UpdateSettings request, CancellationToken cancellationToken)
		{
			throw new System.NotImplementedException();
		}
	}
}
