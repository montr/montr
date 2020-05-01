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
		private readonly INumberGenerator _numerator;

		public CreateClassifierHandler(
			IClassifierTypeService classifierTypeService,
			INumberGenerator numerator)
		{
			_classifierTypeService = classifierTypeService;
			_numerator = numerator;
		}

		public async Task<Classifier> Handle(CreateClassifier request, CancellationToken cancellationToken)
		{
			var type = await _classifierTypeService.Get(request.TypeCode, cancellationToken);

			var number = await _numerator.GenerateNumber(new GenerateNumberRequest
			{
				EntityTypeCode = Classifier.EntityTypeCode,
				EntityTypeUid = type.Uid
			}, cancellationToken);

			return new Classifier
			{
				ParentUid = request.ParentUid,
				Code = number
			};
		}
	}
}
