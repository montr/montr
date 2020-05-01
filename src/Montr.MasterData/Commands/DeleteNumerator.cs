using System;
using MediatR;
using Montr.Core.Models;

namespace Montr.MasterData.Commands
{
	public class DeleteNumerator : IRequest<ApiResult>
	{
		public Guid[] Uids { get; set; }
	}
}
