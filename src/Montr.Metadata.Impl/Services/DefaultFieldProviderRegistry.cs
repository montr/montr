using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Montr.Metadata.Models;
using Montr.Metadata.Services;

namespace Montr.Metadata.Impl.Services
{
	public class DefaultFieldProviderRegistry : IFieldProviderRegistry
	{
		private readonly ConcurrentDictionary<string, IFieldProvider> _providerMap = new ConcurrentDictionary<string, IFieldProvider>();

		public void AddFieldType(string fieldType, IFieldProvider fieldProvider)
		{
			_providerMap[fieldType] = fieldProvider;
		}

		public IFieldProvider GetFieldTypeProvider(string fieldType)
		{
			return _providerMap[fieldType];
		}

		public IList<FieldType> GetFieldTypes()
		{
			return _providerMap.Keys.OrderBy(x => x).Select(x => new FieldType { Code = x, Name = x }).ToList();
		}
	}
}
