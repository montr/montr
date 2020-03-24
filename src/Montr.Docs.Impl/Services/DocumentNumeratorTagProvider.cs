using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Montr.Docs.Models;
using Montr.MasterData.Models;
using Montr.MasterData.Services;

namespace Montr.Docs.Impl.Services
{
	public class DocumentNumberTagResolver : INumberTagResolver
	{
		public bool Supports(GenerateNumberRequest request, out string[] supportedTags)
		{
			if (request.EntityTypeCode == DocumentType.EntityTypeCode)
			{
				supportedTags = new []
				{
					"DocumentType",
					"Company"
				};

				return true;
			}

			supportedTags = null;
			return false;
		}

		public Task Resolve(GenerateNumberRequest request, out DateTime? date,
			IEnumerable<string> tags, IDictionary<string, string> values, CancellationToken cancellationToken)
		{
			// todo: load document properties
			date = DateTime.Today;
			return Task.CompletedTask;
		}
	}
}
