﻿using System;
using MediatR;
using Montr.Core.Models;

namespace Montr.MasterData.Commands
{
	public class DeleteClassifierTree : IRequest<ApiResult>
	{
		public Guid UserUid { get; set; }

		public string TypeCode { get; set; }

		public Guid[] Uids { get; set; }
	}
}
