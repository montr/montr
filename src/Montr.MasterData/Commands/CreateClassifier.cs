using System;
using MediatR;
using Montr.MasterData.Models;

namespace Montr.MasterData.Commands
{
	public class CreateClassifier : ClassifierCreateRequest, IRequest<Classifier>
	{
	}
}
