using System;
using MediatR;
using Montr.MasterData.Models;

namespace Montr.MasterData.Commands
{
	public class CreateClassifier : IRequest<Classifier>
	{
		public Guid CompanyUid { get; set; }

		public Guid UserUid { get; set; }

		public string TypeCode { get; set; }

		public Guid? ParentUid { get; set; }
	}
}
