﻿using System.Collections.Generic;
using System.Linq;
using Montr.MasterData.Models;
using Montr.Metadata.Models;
using Montr.Metadata.Services.Implementations;

namespace Montr.MasterData.Services.Implementations
{
	public class ClassifierFieldProvider : DefaultFieldProvider<ClassifierField, string>
	{
		public override IList<FieldMetadata> GetDesignerMetadata()
		{
			var baseFields = base.GetDesignerMetadata();

			var additionalFields = new List<FieldMetadata>
			{
				new ClassifierTypeField { Key = PropsPrefix + ".typeCode", Required = true, Name = "Type" }
			};

			return baseFields.Union(additionalFields).ToList();
		}
	}

	public class ClassifierGroupFieldProvider : DefaultFieldProvider<ClassifierGroupField, string>
	{
		public override IList<FieldMetadata> GetDesignerMetadata()
		{
			var baseFields = base.GetDesignerMetadata();

			var additionalFields = new List<FieldMetadata>
			{
				new ClassifierTypeField { Key = PropsPrefix + ".typeCode", Required = true, Name = "Type" },
				new TextField { Key = PropsPrefix + ".treeCode", Required = true, Name = "Tree" }
			};

			return baseFields.Union(additionalFields).ToList();
		}
	}
}
