using System;
using System.Collections.Generic;
using MediatR;

namespace Montr.Settings.Events
{
	public class SettingsChanged : INotification
	{
		public string EntityTypeCode { get; set; }

		public Guid EntityUid { get; set; }

		public ICollection<(string, object)> Values { get; set; }
	}
}
