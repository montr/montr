using System;
using Montr.MasterData.Services.Designers;
using Montr.Metadata;

namespace Montr.Idx
{
	public class UserRegistrationOptions
	{
		[FieldDesigner(typeof(ClassifierFieldDesigner))]
		[ClassifierField(ClassifierTypeCode.Role)]
		public Guid[] DefaultRoles { get; set; }
	}
}
