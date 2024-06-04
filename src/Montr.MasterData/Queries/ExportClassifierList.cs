using MediatR;
using Montr.Core.Models;
using Montr.MasterData.Models;

namespace Montr.MasterData.Queries
{
	public class ExportClassifierList : ClassifierSearchRequest, IRequest<FileResult>
	{
		public bool AutoFitColumns { get; set; } = true;
	}
}
