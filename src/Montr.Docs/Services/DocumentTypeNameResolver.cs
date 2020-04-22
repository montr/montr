using System;
using Montr.Core.Services;
using Montr.Docs.Models;

namespace Montr.Docs.Services
{
	public class DocumentTypeNameResolver : IEntityNameResolver
	{
		public string Resolve(string entityTypeCode, Guid entityUid)
		{
			if (entityUid == Process.CompanyRegistrationRequest)
			{
				return "Процесс регистрации (по умолчанию)";
			}

			return null;
		}
	}
}
