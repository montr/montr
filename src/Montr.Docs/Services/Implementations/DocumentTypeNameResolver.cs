using System;
using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Services;
using Montr.MasterData.Services;

namespace Montr.Docs.Services.Implementations;

public class DocumentTypeNameResolver : IEntityNameResolver
{
	private readonly INamedServiceFactory<IClassifierRepository> _classifierRepositoryFactory;

	public DocumentTypeNameResolver(INamedServiceFactory<IClassifierRepository> classifierRepositoryFactory)
	{
		_classifierRepositoryFactory = classifierRepositoryFactory;
	}

	public async Task<string> Resolve(string entityTypeCode, Guid entityUid, CancellationToken cancellationToken)
	{
		var documentTypeRepository = _classifierRepositoryFactory.GetNamedOrDefaultService(ClassifierTypeCode.DocumentType);

		var entity = await documentTypeRepository.Get(ClassifierTypeCode.DocumentType, entityUid, cancellationToken);

		return entity?.Name;

		/*if (entityUid == Process.CompanyRegistrationRequest)
		{
			return "Процесс регистрации (по умолчанию)";
		}*/
	}
}