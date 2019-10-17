using System;
using MediatR;
using Montr.Core.Models;

namespace Montr.MasterData.Commands
{
	public class InsertClassifierLink : IRequest<ApiResult>
	{
		public Guid UserUid { get; set; }

		public Guid CompanyUid { get; set; }

		public string TypeCode { get; set; }

		public Guid GroupUid { get; set; }

		public Guid ItemUid { get; set; }
	}
}
