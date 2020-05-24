using MediatR;

namespace Montr.Core.Events
{
	public class EntityStatusChanged<TEntity> : INotification
	{
		public TEntity Entity { get; set; }

		public string StatusCode { get; set; }
	}
}
