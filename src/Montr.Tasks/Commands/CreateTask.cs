using MediatR;
using Montr.Tasks.Models;

namespace Montr.Tasks.Commands
{
	public class CreateTask : IRequest<TaskModel>
	{
	}
}
