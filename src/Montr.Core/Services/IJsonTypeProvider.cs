using System;

namespace Montr.Core.Services
{
	public interface IJsonTypeProvider
	{
		bool IsRegistered(Type type);
		
		bool TryGetType(string typeCode, out Type type);
	}
}