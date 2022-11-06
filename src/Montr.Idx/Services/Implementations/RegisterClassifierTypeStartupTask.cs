using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Services;
using Montr.Idx.Models;
using Montr.MasterData.Commands;
using Montr.MasterData.Models;
using Montr.MasterData.Services;

namespace Montr.Idx.Services.Implementations
{
	public class RegisterClassifierTypeStartupTask : IStartupTask
	{
		private readonly IClassifierTypeRegistrator _classifierTypeRegistrator;

		public RegisterClassifierTypeStartupTask(IClassifierTypeRegistrator classifierTypeRegistrator)
		{
			_classifierTypeRegistrator = classifierTypeRegistrator;
		}

		public async Task Run(CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			foreach (var command in GetCommands())
			{
				await _classifierTypeRegistrator.Register(command.Item, command.Fields, cancellationToken);
			}
		}

		protected static IEnumerable<RegisterClassifierType> GetCommands()
		{
			yield return Role.GetDefaultMetadata();

			yield return User.GetDefaultMetadata();

			yield return new RegisterClassifierType
			{
				Item = new ClassifierType
				{
					Code = ClassifierTypeCode.Permission,
					Name = "Permissions",
					HierarchyType = HierarchyType.Groups,
					IsSystem = true
				},
				Fields = ClassifierMetadata.GetDefaultFields().ToList()
			};
		}
	}
}
