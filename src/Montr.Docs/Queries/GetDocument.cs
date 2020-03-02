using System;
using MediatR;
using Montr.Docs.Models;

namespace Montr.Docs.Queries
{
	public class GetDocument : IRequest<Document>
	{
		public Guid Uid { get; set; }
	}
}
