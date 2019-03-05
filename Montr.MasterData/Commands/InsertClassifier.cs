using System;
using System.Collections.Generic;
using MediatR;
using Montr.MasterData.Models;

namespace Montr.MasterData.Commands
{
	public class InsertClassifier: IRequest<int>
	{
		public Guid UserUid { get; set; }

		public Guid CompanyUid { get; set; }

		public string TypeCode { get; set; }

		public ICollection<Classifier> Items { get; set; }
	}
}
