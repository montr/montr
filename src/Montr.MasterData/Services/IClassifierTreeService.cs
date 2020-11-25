using System;
using System.Threading;
using System.Threading.Tasks;
using Montr.MasterData.Models;

namespace Montr.MasterData.Services
{
	public interface IClassifierTreeService
	{
		Task<ClassifierTree> GetClassifierTree(/*Guid companyUid,*/ string typeCode, string treeCode, CancellationToken cancellationToken);

		Task<ClassifierTree> GetClassifierTree(/*Guid companyUid,*/ string typeCode, Guid treeUid, CancellationToken cancellationToken);
	}
}
