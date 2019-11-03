using System.Collections.Generic;
using MediatR;
using Montr.Idx.Models;

namespace Montr.Idx.Queries
{
	public class GetAuthSchemes : IRequest<IList<AuthScheme>>
	{
	}
}
