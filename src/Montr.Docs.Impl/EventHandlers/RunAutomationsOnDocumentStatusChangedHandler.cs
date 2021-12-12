using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Montr.Automate.Commands;
using Montr.Core.Events;
using Montr.Docs.Models;
using Montr.Worker.Services;

namespace Montr.Docs.Impl.EventHandlers
{
	public class RunAutomationsOnDocumentStatusChangedHandler : INotificationHandler<EntityStatusChanged<Document>>
	{
		private readonly ILogger<RunAutomationsOnDocumentStatusChangedHandler> _logger;
		private readonly IBackgroundJobManager _jobManager;

		public RunAutomationsOnDocumentStatusChangedHandler(
			ILogger<RunAutomationsOnDocumentStatusChangedHandler> logger, IBackgroundJobManager jobManager)
		{
			_logger = logger;
			_jobManager = jobManager;
		}

		public Task Handle(EntityStatusChanged<Document> notification, CancellationToken cancellationToken)
		{
			var document = notification.Entity;

			// todo: auto-approve request, notifications
			var jobId = _jobManager.Enqueue<ISender>(x => x.Send(new RunAutomations
			{
				MetadataEntityTypeCode = MasterData.EntityTypeCode.Classifier,
				MetadataEntityUid = document.DocumentTypeUid,
				EntityTypeCode = EntityTypeCode.Document,
				EntityUid = document.Uid.Value
			}, cancellationToken));

			return Task.CompletedTask;
		}
	}
}
