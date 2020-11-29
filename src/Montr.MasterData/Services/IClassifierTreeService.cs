using System;
using System.Threading;
using System.Threading.Tasks;
using Montr.MasterData.Models;

namespace Montr.MasterData.Services
{
	public interface IClassifierTreeService
	{
		Task<ClassifierTree> GetClassifierTree(string typeCode, string treeCode, CancellationToken cancellationToken);

		Task<ClassifierTree> GetClassifierTree(string typeCode, Guid treeUid, CancellationToken cancellationToken);
	}
}
