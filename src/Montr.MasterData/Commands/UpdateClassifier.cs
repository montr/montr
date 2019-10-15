using System;
using MediatR;
using Montr.Core.Models;
using Montr.MasterData.Models;

namespace Montr.MasterData.Commands
{
	public class UpdateClassifier: IRequest<ApiResult>
	{
		public Guid UserUid { get; set; }

		public Guid CompanyUid { get; set; }

		public string TypeCode { get; set; }

		public Classifier Item { get; set; }
	}
}
