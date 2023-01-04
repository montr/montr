using System;
using System.ComponentModel.DataAnnotations;
using Montr.MasterData.Services.Designers;
using Montr.Metadata;

namespace Montr.Tasks
{
	public class TasksOptions
	{
		[Required]
		[FieldDesigner(typeof(ClassifierFieldDesigner))]
		[ClassifierField(MasterData.ClassifierTypeCode.Numerator)]
		public Guid? DefaultNumeratorId { get; set; }
	}
}
