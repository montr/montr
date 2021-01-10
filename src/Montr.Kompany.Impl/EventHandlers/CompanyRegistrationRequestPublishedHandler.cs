using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Montr.Core.Events;
using Montr.Docs.Models;

namespace Montr.Kompany.Impl.EventHandlers
{
	public class CompanyRegistrationRequestPublishedHandler : INotificationHandler<EntityStatusChanged<Document>>
	{
		private readonly ILogger<CompanyRegistrationRequestPublishedHandler> _logger;

		public CompanyRegistrationRequestPublishedHandler(ILogger<CompanyRegistrationRequestPublishedHandler> logger)
		{
			_logger = logger;
		}

		public Task Handle(EntityStatusChanged<Document> notification, CancellationToken cancellationToken)
		{
			// todo: wtf, what this handler for?

			return Task.CompletedTask;
		}
	}
}
