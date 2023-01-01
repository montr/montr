using System;
using Montr.MasterData.Services.Designers;
using Montr.Metadata;
using Montr.Settings;

namespace Montr.Kompany.Registration
{
	public class CompanyRegistrationOptions
	{
		[FieldDesigner(typeof(ClassifierFieldDesigner))]
		[ClassifierField(Docs.ClassifierTypeCode.Questionnaire)]
		public Guid? QuestionnaireId { get; set; }
	}
}
