namespace Montr.Core.Models
{
	public class EntityTypeCode
	{
		public static string[] GetRegisteredEntityTypeCodes()
		{
			// todo: replace hardcoded options, should be entity types registered at startup,
			// e.g. classifier, document, task etc.
			return new[] {"classifier", "document", "task"};
		}
	}
}
