using System.Collections.Generic;
using Montr.Metadata.Models;

namespace Montr.MasterData.Models
{
	public class ClassifierMetadata
	{
		public static IEnumerable<FieldMetadata> GetDefaultFields()
		{
			return new FieldMetadata[]
			{
				new TextField { Key = "code", Name = "Код", Required = true, Active = true, DisplayOrder = 10, System = true },
				new TextAreaField { Key = "name", Name = "Наименование", Required = true, Active = true, DisplayOrder = 20, System = true, Props = new TextAreaField.Properties { Rows = 10 } }
			};
		}
	}
}
