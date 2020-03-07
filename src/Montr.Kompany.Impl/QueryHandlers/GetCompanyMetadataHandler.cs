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
using Montr.Metadata.Models;

namespace Montr.Kompany.Impl.QueryHandlers
{
	public class GetCompanyMetadataHandler : IRequestHandler<GetCompanyMetadata, DataView>
	{
		private readonly IRepository<FieldMetadata> _metadataRepository;

		public GetCompanyMetadataHandler(
			IRepository<FieldMetadata> metadataRepository)
		{
			_metadataRepository = metadataRepository;
		}

		public async Task<DataView> Handle(GetCompanyMetadata request, CancellationToken cancellationToken)
		{
			var metadata = await _metadataRepository.Search(new MetadataSearchRequest
			{
				EntityTypeCode = Process.EntityTypeCode,
				EntityUid = Process.CompanyRegistrationRequest,
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
