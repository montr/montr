using Montr.Core.Services;

namespace Montr.Metadata.Models
{
	public class FieldKey
	{
		public static readonly string KeyPrefix = ExpressionHelper.GetMemberName<IFieldDataContainer>(x => x.Fields).ToLowerInvariant();

		public static string FormatFullKey(string key)
		{
			return $"{KeyPrefix}.{key}";
		}
	}
}
