using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Core.Commands;
using Montr.Core.Models;
using Montr.Core.Services.Implementations;

namespace Montr.Core.Services.CommandHandlers
{
	public class ImportLocaleStringListHandler : IRequestHandler<ImportLocaleStringList, ApiResult>
	{
		private readonly IUnitOfWorkFactory _unitOfWorkFactory;
		private readonly LocaleStringSerializer _serializer;
		private readonly ILocaleStringImporter _importer;

		public ImportLocaleStringListHandler(IUnitOfWorkFactory unitOfWorkFactory,
			LocaleStringSerializer serializer, ILocaleStringImporter importer)
		{
			_unitOfWorkFactory = unitOfWorkFactory;
			_serializer = serializer;
			_importer = importer;
		}

		public async Task<ApiResult> Handle(ImportLocaleStringList request, CancellationToken cancellationToken)
		{
			var list = await _serializer.Deserialize(request.File.OpenReadStream(), cancellationToken);

			var affected = 0;

			using (var scope = _unitOfWorkFactory.Create())
			{
				await _importer.Import(list, cancellationToken);

				scope.Commit();
			}

			return new ApiResult { AffectedRows = affected };
		}
	}
}
