using System;
using Montr.Metadata.Models;
using MediatR;

namespace Montr.Kompany.Queries
{
	public class GetCompanyMetadata : IRequest<DataView>
	{
		public Guid UserUid { get; set; }
	}
}
