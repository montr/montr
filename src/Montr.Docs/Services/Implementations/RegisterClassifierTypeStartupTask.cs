using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Services;
using Montr.Docs.Models;
using Montr.MasterData.Commands;
using Montr.MasterData.Models;
using Montr.MasterData.Services;
using Montr.Metadata.Models;

namespace Montr.Docs.Services.Implementations;

public class RegisterClassifierTypeStartupTask : IStartupTask
{
	private readonly IClassifierTypeRegistrator _classifierTypeRegistrator;

	public RegisterClassifierTypeStartupTask(IClassifierTypeRegistrator classifierTypeRegistrator)
	{
		_classifierTypeRegistrator = classifierTypeRegistrator;
	}

	public async Task Run(CancellationToken cancellationToken)
	{
		foreach (var classifierType in GetClassifierTypes())
		{
			await _classifierTypeRegistrator.Register(classifierType.Item, classifierType.Fields, cancellationToken);
		}
	}

	protected static IEnumerable<RegisterClassifierType> GetClassifierTypes()
	{
		yield return DocumentType.GetDefaultMetadata();

		yield return new RegisterClassifierType
		{
			Item = new ClassifierType
			{
				Code = ClassifierTypeCode.Questionnaire,
				Name = "Questionnaires",
				HierarchyType = HierarchyType.Groups,
				IsSystem = true
			},
			Fields = new List<FieldMetadata>
			{
				new TextField { Key = "code", Name = "Code", Required = true, DisplayOrder = 10, System = true },
				new TextAreaField { Key = "name", Name = "Name", Required = true, DisplayOrder = 20, System = true, Props = new TextAreaField.Properties { Rows = 2 } },
			}
		};

		yield return new RegisterClassifierType
		{
			Item = new ClassifierType
			{
				Code = ClassifierTypeCode.Process,
				Name = "Processes",
				HierarchyType = HierarchyType.Groups,
				IsSystem = true
			},
			Fields = new List<FieldMetadata>
			{
				new TextField { Key = "code", Name = "Code", Required = true, DisplayOrder = 10, System = true },
				new TextAreaField { Key = "name", Name = "Name", Required = true, DisplayOrder = 20, System = true, Props = new TextAreaField.Properties { Rows = 2 } },
			}
		};
	}
}
