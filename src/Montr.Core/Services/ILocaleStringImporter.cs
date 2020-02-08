using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Models;

namespace Montr.Core.Services
{
	public interface ILocaleStringImporter
	{
		Task Import(IList<LocaleStringList> list, CancellationToken cancellationToken);
	}
}
