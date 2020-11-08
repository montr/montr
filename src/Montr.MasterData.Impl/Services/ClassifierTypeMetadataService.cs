using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Services;
using Montr.MasterData.Models;
using Montr.MasterData.Services;
using Montr.Metadata.Models;

namespace Montr.MasterData.Impl.Services
{
	public class ClassifierTypeMetadataService : IClassifierTypeMetadataService
	{
		private readonly IRepository<FieldMetadata> _fieldMetadataRepository;

		public ClassifierTypeMetadataService(IRepository<FieldMetadata> fieldMetadataRepository)
		{
			_fieldMetadataRepository = fieldMetadataRepository;
		}

		public async Task<IList<FieldMetadata>> GetMetadata(ClassifierType type, CancellationToken cancellationToken)
		{
			var metadata = await _fieldMetadataRepository.Search(new MetadataSearchRequest
			{
				EntityTypeCode = ClassifierType.TypeCode,
				EntityUid = type.Uid,
				// todo: check flags
				// IsSystem = false,
				IsActive = true,
				SkipPaging = true
			}, cancellationToken);

			return metadata.Rows;
		}
	}
}
