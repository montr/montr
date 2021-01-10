using System.Collections.Generic;
using MediatR;

namespace Montr.Core.Events
{
	public class SettingsChanged : INotification
	{
		public ICollection<(string, object)> Values { get; set; }
	}
}
