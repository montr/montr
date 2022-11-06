using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Services;
using Montr.Docs.Models;
using Montr.Kompany.Models;
using Montr.MasterData.Models;
using Montr.MasterData.Services;

namespace Montr.Kompany.Registration.Services.Implementations
{
	public class RegisterClassifiersStartupTask : IStartupTask
	{
		private readonly IClassifierRegistrator _registrator;

		public RegisterClassifiersStartupTask(IClassifierRegistrator registrator)
		{
			_registrator = registrator;
		}

		public async Task Run(CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			foreach (var item in GetClassifiers())
			{
				await _registrator.Register(item, cancellationToken);
			}
		}

		// todo: register document types numerators
		// todo: move to recipes
		public static IEnumerable<Classifier> GetClassifiers()
		{
			yield return new DocumentType
			{
				Code = DocumentTypes.CompanyRegistrationRequest,
				Name = "Company registration request",
				IsSystem = true
			};

			yield return new DocumentType
			{
				Code = DocumentTypes.CompanyChangeRequest,
				Name = "Company change request",
				IsSystem = true
			};
		}
	}
}
