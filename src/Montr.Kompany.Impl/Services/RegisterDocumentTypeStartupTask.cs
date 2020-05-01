using System.Collections.Generic;
using MediatR;
using Microsoft.Extensions.Logging;
using Montr.Docs.Commands;
using Montr.Docs.Models;
using Montr.Docs.Services;

namespace Montr.Kompany.Impl.Services
{
	public class RegisterDocumentTypeStartupTask : AbstractRegisterDocumentTypeStartupTask
	{
		public RegisterDocumentTypeStartupTask(ILogger<RegisterDocumentTypeStartupTask> logger, IMediator mediator) : base(logger, mediator)
		{
		}

		protected override IEnumerable<RegisterDocumentType> GetCommands()
		{
			yield return new RegisterDocumentType
			{
				Item = new DocumentType
				{
					Code = "company_registration_request",
					Name = "Заявка на регистацию компании",
					// IsSystem = true
				}
			};
		}
	}
}
