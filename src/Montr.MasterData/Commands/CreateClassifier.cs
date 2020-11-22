using System;
using MediatR;
using Montr.MasterData.Models;

namespace Montr.MasterData.Commands
{
	public class CreateClassifier : ClassifierCreateRequest, IRequest<Classifier>
	{
		public Guid CompanyUid { get; set; }

		public Guid UserUid { get; set; }
	}
}
