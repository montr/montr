using System;
using MediatR;
using Montr.Core.Models;
using Montr.MasterData.Models;

namespace Montr.MasterData.Commands
{
	public class InsertClassifier : IRequest<ApiResult>
	{
		public Guid UserUid { get; set; }

		public Classifier Item { get; set; }
	}
}
