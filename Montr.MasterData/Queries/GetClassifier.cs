using System;
using MediatR;
using Montr.MasterData.Models;

namespace Montr.MasterData.Queries
{
	public class GetClassifier : IRequest<Classifier>
	{
		public Guid UserUid { get; set; }

		public Guid EntityUid { get; set; }
	}
}
