﻿using System.Collections.Generic;
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
		public class SupportedTags
		{
			public static readonly string DocumentType = "DocumentType";

			public static readonly string Company = "Company";
		}

		private readonly IRepository<Document> _repository;
		private readonly INamedServiceFactory<IClassifierRepository> _classifierRepositoryFactory;

		public DocumentNumberTagResolver(IRepository<Document> repository, INamedServiceFactory<IClassifierRepository> classifierRepositoryFactory)
		{
			_repository = repository;
			_classifierRepositoryFactory = classifierRepositoryFactory;
		}

		public bool Supports(GenerateNumberRequest request, out string[] supportedTags)
		{
			if (request.EntityTypeCode == DocumentType.EntityTypeCode)
			{
				supportedTags = new [] { SupportedTags.DocumentType, SupportedTags.Company };

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
					DocumentType documentType = null;

					var result = new NumberTagResolveResult
					{
						Date = document.DocumentDate,
						Values = new Dictionary<string, string>(Numerator.TagComparer)
					};

					foreach (var tag in tags)
					{
						// todo: read document type and company number tags from separate table (service)
						if (tag == SupportedTags.DocumentType)
						{
							if (documentType == null)
							{
								var classifierRepository = _classifierRepositoryFactory.GetNamedOrDefaultService(ClassifierTypeCode.DocumentType);
								documentType = (DocumentType)await classifierRepository.Get(ClassifierTypeCode.DocumentType, document.DocumentTypeUid, cancellationToken);
							}

							result.Values[tag] = "CRR"; // documentType.Code;
						}
						else if (tag == SupportedTags.Company)
						{
							// todo: resolve tag
						}
					}

					return result;
				}
			}

			return null;
		}
	}
}
