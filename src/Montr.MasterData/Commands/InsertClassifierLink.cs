using System;
using MediatR;
using Montr.MasterData.Models;
using Montr.Metadata.Models;

namespace Montr.MasterData.Commands
{
	public class InsertClassifierLink : IRequest<ApiResult>
	{
		public Guid UserUid { get; set; }

		public Guid CompanyUid { get; set; }

		public string TypeCode { get; set; }

		public ClassifierLink Item { get; set; }
	}
}