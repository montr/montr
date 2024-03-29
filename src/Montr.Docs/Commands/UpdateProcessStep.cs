﻿using System;
using MediatR;
using Montr.Core.Models;
using Montr.Docs.Models;

namespace Montr.Docs.Commands
{
	public class UpdateProcessStep : IRequest<ApiResult>
	{
		public Guid ProcessUid { get; set; }

		public ProcessStep Item { get; set; }
	}
}