using System;
using MediatR;
using Montr.Core.Models;

namespace Montr.Docs.Commands;

public class SubmitDocument : IRequest<ApiResult>
{
	public Guid? DocumentUid { get; set; }
}