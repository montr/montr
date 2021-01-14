using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Models;

namespace Montr.Core.Services
{
	/// <summary>
	/// todo: move to separate app?
	/// </summary>
	/// <see cref="https://www.enterpriseready.io/features/audit-log/"/>
	/// <see cref="http://support.sugarcrm.com/Knowledge_Base/User_Interface/Historical_Summary_vs._Activity_Stream_vs._Change_Log/"/>
	public interface IAuditLogService
	{
		Task Save(AuditEvent entry, CancellationToken cancellationToken);
	}
}
