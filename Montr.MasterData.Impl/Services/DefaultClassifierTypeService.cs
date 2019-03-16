﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Services;
using Montr.MasterData.Models;
using Montr.MasterData.Services;

namespace Montr.MasterData.Impl.Services
{
	public class DefaultClassifierTypeService: IClassifierTypeService
	{
		private readonly IRepository<ClassifierType> _classifierTypeRepository;

		public DefaultClassifierTypeService(IRepository<ClassifierType> classifierTypeRepository)
		{
			_classifierTypeRepository = classifierTypeRepository;
		}

		public async Task<ClassifierType> GetClassifierType(Guid companyUid, string typeCode, CancellationToken cancellationToken)
		{
			var types = await _classifierTypeRepository
				.Search(new ClassifierTypeSearchRequest
				{
					CompanyUid = companyUid,
					Code = typeCode
				}, cancellationToken);

			return types.Rows.Single();
		}
	}
}
