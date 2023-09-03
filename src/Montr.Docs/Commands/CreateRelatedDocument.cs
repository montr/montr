using System;
using System.Security.Claims;
using MediatR;
using Montr.Core.Models;
using Montr.Metadata.Models;

namespace Montr.Docs.Commands
{
	public class CreateRelatedDocument : Button, IRequest<ApiResult>
	{
		public ClaimsPrincipal Principal { get; set; }

		public Guid UserUid { get; set; }

		public Guid? DocumentUid { get; set; }

		public string DocumentTypeCode { get; set; }

		public string RelationTypeCode { get; set; }
	}
}
