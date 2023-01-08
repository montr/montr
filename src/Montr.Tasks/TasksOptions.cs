using System;
using Montr.MasterData.Services.Designers;
using Montr.Metadata;

namespace Montr.Tasks
{
	public class TasksOptions
	{
		[FieldDesigner(typeof(ClassifierFieldDesigner))]
		[ClassifierField(MasterData.ClassifierTypeCode.Numerator)]
		public Guid? DefaultNumeratorId { get; set; }
	}
}
