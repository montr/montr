using System;
using Montr.MasterData.Services.Designers;
using Montr.Settings;

namespace Montr.Kompany.Registration
{
	public class CompanyRegistrationOptions
	{
		[SettingsDesigner(typeof(ClassifierFieldSettingsDesigner))]
		[ClassifierField(Docs.ClassifierTypeCode.Questionnaire)]
		public Guid? QuestionnaireId { get; set; }
	}
}
