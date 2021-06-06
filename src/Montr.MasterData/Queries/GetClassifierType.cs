using System;
using MediatR;
using Montr.MasterData.Models;

namespace Montr.MasterData.Queries
{
	public class GetClassifierType : IRequest<ClassifierType>
	{
		public string TypeCode { get; set; }

		public Guid? Uid { get; set; }
	}
}
