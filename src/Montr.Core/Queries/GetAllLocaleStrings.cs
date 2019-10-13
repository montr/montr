using System.Collections.Generic;
using MediatR;

namespace Montr.Core.Queries
{
	public class GetAllLocaleStrings : IRequest<IDictionary<string, string>>
	{
		public string Locale { get; set; }

		public string Module { get; set; }
	}
}
