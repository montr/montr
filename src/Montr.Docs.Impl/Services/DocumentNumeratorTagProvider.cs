using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Services;
using Montr.Docs.Models;
using Montr.MasterData.Models;
using Montr.MasterData.Services;

namespace Montr.Docs.Impl.Services
{
	public class DocumentNumberTagResolver : INumberTagResolver
	{
		private readonly IRepository<Document> _repository;

		public DocumentNumberTagResolver(IRepository<Document> repository)
		{
			_repository = repository;
		}

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

		public async Task<NumberTagResolveResult> Resolve(GenerateNumberRequest request, IEnumerable<string> tags, CancellationToken cancellationToken)
		{
			if (request.EntityTypeCode == DocumentType.EntityTypeCode)
			{
				var searchResult = await _repository.Search(new DocumentSearchRequest { Uid = request.EntityUid }, cancellationToken);

				var document = searchResult.Rows.FirstOrDefault();

				if (document != null)
				{
					var result = new NumberTagResolveResult
					{
						Date = document.DocumentDate,
						Values = new Dictionary<string, string>(Numerator.TagComparer)
					};

					foreach (var tag in tags)
					{
						if (tag == "DocumentType")
						{
							result.Values[tag] = "CRR"; // todo: read from document
						}
					}

					return result;
				}
			}

			return null;
		}
	}
}
