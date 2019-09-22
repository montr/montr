using System;
using MediatR;
using Montr.Core.Models;
using Montr.MasterData.Models;

namespace Montr.MasterData.Queries
{
	public class ExportClassifierList : IRequest<FileResult>
	{
		public ClassifierSearchRequest Request { get; set; }
	}
}
