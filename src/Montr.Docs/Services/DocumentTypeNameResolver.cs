using System;
using Montr.Core.Services;

namespace Montr.Docs.Services
{
	public class DocumentTypeNameResolver : IEntityNameResolver
	{
		public string Resolve(string entityTypeCode, Guid entityUid)
		{
			if (entityUid == Guid.Parse("8c41ebdc-e176-424e-9048-249e9862dbb2"))
			{
				return "Процесс регистрации (по умолчанию)";
			}

			return null;
		}
	}
}
