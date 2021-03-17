using System;
using MediatR;
using Montr.MasterData.Models;

namespace Montr.MasterData.Queries
{
	public class GetClassifier : IRequest<Classifier>
	{
		public Guid UserUid { get; set; }

		public string TypeCode { get; set; }

		public Guid Uid { get; set; }
	}
}
