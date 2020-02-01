using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Metadata.Commands;
using Montr.Metadata.Models;
using Montr.Metadata.Services;

namespace Montr.Metadata.Impl.CommandHandlers
{
	public class InsertDataFieldHandler : IRequestHandler<InsertDataField, ApiResult>
	{
		private readonly IUnitOfWorkFactory _unitOfWorkFactory;
		private readonly IFieldMetadataService _metadataService;

		public InsertDataFieldHandler(IUnitOfWorkFactory unitOfWorkFactory, IFieldMetadataService metadataService)
		{
			_unitOfWorkFactory = unitOfWorkFactory;
			_metadataService = metadataService;
		}

		public async Task<ApiResult> Handle(InsertDataField request, CancellationToken cancellationToken)
		{
			var item = request.Item ?? throw new ArgumentNullException(nameof(request.Item));

			using (var scope = _unitOfWorkFactory.Create())
			{
				// todo: merge InsertDataField with ManageFieldMetadataRequest
				var result = await _metadataService.Insert(new ManageFieldMetadataRequest
				{
					EntityTypeCode = request.EntityTypeCode,
					EntityUid = request.EntityUid,
					Item = item
				}, cancellationToken);

				if (result.Success) scope.Commit();

				return result;
			}
		}
	}
}
