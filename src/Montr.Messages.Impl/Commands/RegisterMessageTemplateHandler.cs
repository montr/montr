using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Messages.Commands;
using Montr.Messages.Services;

namespace Montr.Messages.Impl.Commands
{
	public class RegisterMessageTemplateHandler : IRequestHandler<RegisterMessageTemplate, ApiResult>
	{
		private readonly IUnitOfWorkFactory _unitOfWorkFactory;
		private readonly IMessageTemplateService _messageTemplateService;

		public RegisterMessageTemplateHandler(
			IUnitOfWorkFactory unitOfWorkFactory,
			IMessageTemplateService messageTemplateService)
		{
			_unitOfWorkFactory = unitOfWorkFactory;
			_messageTemplateService = messageTemplateService;
		}

		public async Task<ApiResult> Handle(RegisterMessageTemplate request, CancellationToken cancellationToken)
		{
			var item = request.Item ?? throw new ArgumentNullException(nameof(request.Item));

			// todo: use codes
			var type = await _messageTemplateService.TryGet(item.Uid, cancellationToken);
			
			if (type != null) return new ApiResult { AffectedRows = 0 };

			using (var scope = _unitOfWorkFactory.Create())
			{
				var insertTypeResult = await _messageTemplateService.Insert(item, cancellationToken);

				if (insertTypeResult.Success == false) return insertTypeResult;

				scope.Commit();

				return new ApiResult { AffectedRows = 1, Uid = insertTypeResult.Uid };
			}
		}
	}
}
