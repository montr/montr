using System;
using Montr.Metadata.Services;

namespace Montr.Metadata.Impl.Services
{
	public class DefaultFieldProvider<TFieldType> : IFieldProvider
	{
		public Type FieldType => typeof(TFieldType);
	}
}
