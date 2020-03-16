using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Core.Services;
using Montr.MasterData.Commands;
using Montr.MasterData.Models;
using Montr.MasterData.Services;

namespace Montr.MasterData.Impl.CommandHandlers
{
	public class CreateClassifierHandler : IRequestHandler<CreateClassifier, Classifier>
	{
		private readonly IClassifierTypeService _classifierTypeService;
		private readonly INumberingService _numberingService;
		private readonly INamedServiceFactory<INumeratorTagProvider> _tagProviderFactory;

		public CreateClassifierHandler(
			IClassifierTypeService classifierTypeService,
			INumberingService numberingService,
			INamedServiceFactory<INumeratorTagProvider> tagProviderFactory)
		{
			_classifierTypeService = classifierTypeService;
			_numberingService = numberingService;
			_tagProviderFactory = tagProviderFactory;
		}

		public async Task<Classifier> Handle(CreateClassifier request, CancellationToken cancellationToken)
		{
			var type = await _classifierTypeService.Get(request.TypeCode, cancellationToken);

			var numeratorTagProvider = _tagProviderFactory.Resolve(ClassifierType.EntityTypeCode);

			var number = await _numberingService.GenerateNumber(ClassifierType.EntityTypeCode, type.Uid, cancellationToken);

			return new Classifier
			{
				ParentUid = request.ParentUid,
				Code = number
			};
		}
	}
}
