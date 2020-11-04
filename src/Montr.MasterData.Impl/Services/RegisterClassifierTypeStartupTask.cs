using System.Collections.Generic;
using MediatR;
using Microsoft.Extensions.Logging;
using Montr.MasterData.Commands;
using Montr.MasterData.Models;
using Montr.MasterData.Services;
using Montr.Metadata.Models;

namespace Montr.MasterData.Impl.Services
{
	public class RegisterClassifierTypeStartupTask : AbstractRegisterClassifierTypeStartupTask
	{
		public RegisterClassifierTypeStartupTask(ILogger<RegisterClassifierTypeStartupTask> logger, IMediator mediator) : base(logger, mediator)
		{
		}

		protected override IEnumerable<RegisterClassifierType> GetCommands()
		{
			yield return new RegisterClassifierType
			{
				Item = new ClassifierType
				{
					Code = "numerator",
					Name = "Нумераторы",
					HierarchyType = HierarchyType.Groups,
					IsSystem = true
				},
				Fields = new List<FieldMetadata>
				{
					// todo: remove "Numerator/Grid" and "Numerator/Form" from RegisterClassifierMetadataStartupTask
					// new TextField { Key = "code", Name = "Код", Required = true, Active = true, DisplayOrder = 10, System = true },
					new TextAreaField { Key = "name", Name = "Наименование", Required = true, Active = true, System = true, Props = new TextAreaField.Properties { Rows = 2 } },
					new SelectField
					{
						Key = "entityTypeCode", Name = "EntityTypeCode", Required = true,
						Props =
						{
							Options = new []
							{
								new SelectFieldOption { Value = "DocumentType", Name = "DocumentType" },
								new SelectFieldOption { Value = "ClassifierType", Name = "ClassifierType" },
							}
						}
					},
					new SelectField
					{
						Key = "periodicity", Name = "Periodicity", Required = true,
						Props =
						{
							Options = new []
							{
								new SelectFieldOption { Value = "None", Name = "None" },
								new SelectFieldOption { Value = "Day", Name = "Day" },
								new SelectFieldOption { Value = "Month", Name = "Month" },
								new SelectFieldOption { Value = "Quarter", Name = "Quarter" },
								new SelectFieldOption { Value = "Year", Name = "Year" },
							}
						}
					},
					new TextAreaField { Key = "pattern", Name = "Pattern", Required = true, Props = new TextAreaField.Properties { Rows = 4 } },
				}
			};
		}
	}
}
