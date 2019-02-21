using System;
using MediatR;
using Montr.MasterData.Models;

namespace Montr.MasterData.Commands
{
	public class UpdateClassifier: IRequest
	{
		public Guid UserUid { get; set; }

		public Guid CompanyUid { get; set; }

		public Classifier Item { get; set; }
	}
}
