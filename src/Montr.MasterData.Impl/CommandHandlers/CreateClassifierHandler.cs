using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.MasterData.Commands;
using Montr.MasterData.Models;
using Montr.MasterData.Services;

namespace Montr.MasterData.Impl.CommandHandlers
{
	public class CreateClassifierHandler : IRequestHandler<CreateClassifier, Classifier>
	{
		private readonly IClassifierTypeService _classifierTypeService;
		private readonly INumberingService _numberingService;

		public CreateClassifierHandler(IClassifierTypeService classifierTypeService, INumberingService numberingService)
		{
			_classifierTypeService = classifierTypeService;
			_numberingService = numberingService;
		}

		public async Task<Classifier> Handle(CreateClassifier request, CancellationToken cancellationToken)
		{
			var type = await _classifierTypeService.Get(request.TypeCode, cancellationToken);

			var number = await _numberingService.GenerateNumber(ClassifierType.EntityTypeCode, type.Uid, cancellationToken);

			return new Classifier
			{
				ParentUid = request.ParentUid,
				Code = number
			};
		}
	}
}
