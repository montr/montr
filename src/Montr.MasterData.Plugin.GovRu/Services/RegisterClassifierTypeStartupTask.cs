using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Montr.Core.Services;
using Montr.MasterData.Commands;
using Montr.MasterData.Models;
using Montr.Metadata.Models;

namespace Montr.MasterData.Plugin.GovRu.Services
{
	// todo: split to abstract startup task and gov.ru service
	public class RegisterClassifierTypeStartupTask : IStartupTask
	{
		private readonly ILogger<RegisterClassifierTypeStartupTask> _logger;
		private readonly IMediator _mediator;

		public RegisterClassifierTypeStartupTask(
			ILogger<RegisterClassifierTypeStartupTask> logger,
			IMediator mediator)
		{
			_logger = logger;
			_mediator = mediator;
		}

		public async Task Run(CancellationToken cancellationToken)
		{
			foreach (var command in GetCommands())
			{
				var result = await _mediator.Send(command, cancellationToken);

				result.AssertSuccess(() => $"Failed to register classifier type \"{command.Item.Code}\"");

				if (result.AffectedRows == 1)
				{
					_logger.LogInformation("Classifier type {code} successfully registered.", command.Item.Code);
				}
				else
				{
					_logger.LogDebug("Classifier type {code} already registered.", command.Item.Code);
				}
			}
		}

		protected IEnumerable<RegisterClassifierType> GetCommands()
		{
			yield return new RegisterClassifierType
			{
				Item = new ClassifierType
				{
					Code = "okei",
					Name = "ОКЕИ",
					Description = "Общероссийский классификатор единиц измерения",
					HierarchyType = HierarchyType.Groups,
					IsSystem = true
				},
				Fields = new List<FieldMetadata>
				{
					new TextField { Key = "code", Name = "Код", Required = true, Active = true, DisplayOrder = 10, System = true },
					new TextAreaField { Key = "name", Name = "Наименование", Required = true, Active = true, DisplayOrder = 20, System = true, Props = new TextAreaField.Properties { Rows = 10 } },
				}
			};

			yield return new RegisterClassifierType
			{
				Item = new ClassifierType
				{
					Code = "okved2",
					Name = "ОКВЭД 2",
					Description = "Общероссийский классификатор видов экономической деятельности",
					HierarchyType = HierarchyType.Items,
					IsSystem = true
				},
				Fields = new List<FieldMetadata>
				{
					new TextField { Key = "code", Name = "Код", Required = true, Active = true, DisplayOrder = 10, System = true },
					new TextAreaField { Key = "name", Name = "Наименование", Required = true, Active = true, DisplayOrder = 20, System = true, Props = new TextAreaField.Properties { Rows = 10 } },
				}
			};

			yield return new RegisterClassifierType
			{
				Item = new ClassifierType
				{
					Code = "okpd2",
					Name = "ОКПД 2",
					Description = "Общероссийский классификатор продукции по видам экономической деятельности",
					HierarchyType = HierarchyType.Items,
					IsSystem = true
				},
				Fields = new List<FieldMetadata>
				{
					new TextField { Key = "code", Name = "Код", Required = true, Active = true, DisplayOrder = 10, System = true },
					new TextAreaField { Key = "name", Name = "Наименование", Required = true, Active = true, DisplayOrder = 20, System = true, Props = new TextAreaField.Properties { Rows = 10 } },
				}
			};

			yield return new RegisterClassifierType
			{
				Item = new ClassifierType
				{
					Code = "oktmo",
					Name = "ОКТМО",
					Description = "Общероссийский классификатор территорий муниципальных образований",
					HierarchyType = HierarchyType.Items,
					IsSystem = true
				},
				Fields = new List<FieldMetadata>
				{
					new TextField { Key = "code", Name = "Код", Required = true, Active = true, DisplayOrder = 10, System = true },
					new TextAreaField { Key = "name", Name = "Наименование", Required = true, Active = true, DisplayOrder = 20, System = true, Props = new TextAreaField.Properties { Rows = 10 } },
				}
			};

			yield return new RegisterClassifierType
			{
				Item = new ClassifierType
				{
					Code = "okopf",
					Name = "ОКОПФ",
					Description = "Общероссийский классификатор организационно-правовых форм",
					HierarchyType = HierarchyType.Items,
					IsSystem = true
				},
				Fields = new List<FieldMetadata>
				{
					new TextField { Key = "code", Name = "Код", Required = true, Active = true, DisplayOrder = 10, System = true },
					new TextAreaField { Key = "name", Name = "Наименование", Required = true, Active = true, DisplayOrder = 20, System = true, Props = new TextAreaField.Properties { Rows = 10 } },
				}
			};

			yield return new RegisterClassifierType
			{
				Item = new ClassifierType
				{
					Code = "okv",
					Name = "ОКВ",
					Description = "Общероссийский классификатор валют",
					HierarchyType = HierarchyType.Groups,
					IsSystem = true
				},
				Fields = new List<FieldMetadata>
				{
					new TextField { Key = "code", Name = "Код", Required = true, Active = true, DisplayOrder = 10, System = true },
					new TextAreaField { Key = "name", Name = "Наименование", Required = true, Active = true, DisplayOrder = 20, System = true, Props = new TextAreaField.Properties { Rows = 10 } },
					new TextField { Key = "digitalCode", Name = "Цифровой код", Required = true, Active = true, DisplayOrder = 30, System = true },
					new TextField { Key = "shortName", Name = "Краткое наименование", Required = true, Active = true, DisplayOrder = 40, System = true },
				}
			};
		}
	}
}
