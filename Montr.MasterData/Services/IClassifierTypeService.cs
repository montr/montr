using System;
using System.Threading;
using System.Threading.Tasks;
using Montr.MasterData.Models;

namespace Montr.MasterData.Services
{
	public interface IClassifierTypeService
	{
		Task<ClassifierType> GetClassifierType(Guid companyUid, string typeCode, CancellationToken cancellationToken);
	}
}
