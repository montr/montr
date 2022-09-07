using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Montr.MasterData.Commands;
using Montr.MasterData.Models;
using Montr.Metadata.Models;

namespace Montr.Automate.Models
{
	[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
	public class Automation : Classifier
	{
		private string DebuggerDisplay => $"{Name} ({EntityTypeCode})";

		/// <summary>
		/// classifier | document | task ... etc.
		/// </summary>
		public string EntityTypeCode { get; set; }

		/// <summary>
		/// trigger | monitor | schedule (date-based)
		/// </summary>
		public string AutomationTypeCode { get; set; }

		/// <summary>
		/// Meet all conditions...
		/// </summary>
		public IList<AutomationCondition> Conditions { get; set; }

		/// <summary>
		/// ...to execute all actions
		/// </summary>
		public IList<AutomationAction> Actions { get; set; }

		public static RegisterClassifierType GetDefaultMetadata()
		{
			return new RegisterClassifierType
			{
				Item = new ClassifierType
				{
					Code = ClassifierTypeCode.Automation,
					Name = "Automations",
					HierarchyType = HierarchyType.Groups,
					IsSystem = true
				},
				Fields = new List<FieldMetadata>
				{
					new NumberField { Key = "displayOrder", Name = "#", /*Required = true,*/ Props = { Min = 0, Max = 256 }, DisplayOrder = 5 },
					new TextField { Key = "code", Name = "Code", Required = true, DisplayOrder = 10, System = true },
					new TextField { Key = "name", Name = "Наименование", Required = true, DisplayOrder = 20, System = true },
					new TextAreaField { Key = "description", Name = "Описание", Props = new TextAreaField.Properties { Rows = 1 }, DisplayOrder = 30 },
					new SelectField
					{
						Key = "entityTypeCode", Name = "EntityTypeCode", Required = true, DisplayOrder = 40, System = true,
						Props =
						{
							Options = Core.Models.EntityTypeCode
								.GetRegisteredEntityTypeCodes()
								.Select(x => new SelectFieldOption { Value = x, Name = x })
								.ToArray()
						}
					},
					new AutomationConditionListField { Key = "conditions", Name = "Conditions", DisplayOrder = 50, System = true },
					new AutomationActionListField { Key = "actions", Name = "Actions", DisplayOrder = 60, System = true }
				}
			};
		}
	}

	public static class AutomationTypeCode
	{
		public static readonly string Trigger = "trigger";
	}
}
