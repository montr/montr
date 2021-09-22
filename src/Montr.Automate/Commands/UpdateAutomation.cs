using System;
using MediatR;
using Montr.Automate.Models;
using Montr.Core.Models;

namespace Montr.Automate.Commands
{
	public class UpdateAutomation : IRequest<ApiResult>
	{
		public string EntityTypeCode { get; set; }

		public Guid EntityUid { get; set; }

		public Automation Item { get; set; }}
}
