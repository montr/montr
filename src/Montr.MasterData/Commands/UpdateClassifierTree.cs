using System;
using MediatR;
using Montr.Core.Models;
using Montr.MasterData.Models;

namespace Montr.MasterData.Commands
{
	public class UpdateClassifierTree : IRequest<ApiResult>
	{
		public Guid UserUid { get; set; }

		public string TypeCode { get; set; }

		public ClassifierTree Item { get; set; }
	}
}
