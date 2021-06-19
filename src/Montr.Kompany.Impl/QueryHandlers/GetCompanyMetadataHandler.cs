using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Core.Services;
using Montr.Docs.Models;
using Montr.Kompany.Models;
using Montr.Kompany.Queries;
using Montr.MasterData.Services;
using Montr.Metadata.Models;

namespace Montr.Kompany.Impl.QueryHandlers
{
	// todo: why named GetCompanyMetadata, if returns metadata of "company registration request" document?
	public class GetCompanyMetadataHandler : IRequestHandler<GetCompanyMetadata, DataView>
	{
		private readonly INamedServiceFactory<IClassifierRepository> _classifierRepositoryFactory;
		private readonly IRepository<FieldMetadata> _metadataRepository;

		public GetCompanyMetadataHandler(
			INamedServiceFactory<IClassifierRepository> classifierRepositoryFactory,
			IRepository<FieldMetadata> metadataRepository)
		{
			_classifierRepositoryFactory = classifierRepositoryFactory;
			_metadataRepository = metadataRepository;
		}

		public async Task<DataView> Handle(GetCompanyMetadata request, CancellationToken cancellationToken)
		{
			var classifierRepository = _classifierRepositoryFactory.GetNamedOrDefaultService(Docs.ClassifierTypeCode.DocumentType);

			var documentType = await classifierRepository.Get(Docs.ClassifierTypeCode.DocumentType, DocumentTypes.CompanyRegistrationRequest, cancellationToken);

			var metadata = await _metadataRepository.Search(new MetadataSearchRequest
			{
				EntityTypeCode = DocumentType.EntityTypeCode,
				EntityUid = documentType.Uid.Value,
				IsActive = true,
				SkipPaging = true
			}, cancellationToken);

			var dbFields = new List<string>
			{
				nameof(Company.Name)
			};

			var result = new DataView { Fields = metadata.Rows };

			foreach (var field in result.Fields)
			{
				if (dbFields.Contains(field.Key, StringComparer.InvariantCultureIgnoreCase) == false)
				{
					field.Key = FieldKey.FormatFullKey(field.Key);
				}
			}

			return result;
		}
	}
}
