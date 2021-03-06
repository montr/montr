﻿using System;
using MediatR;
using Montr.Core.Models;
using Montr.Tendr.Models;

namespace Montr.Tendr.Commands
{
	public class UpdateEvent : IRequest<ApiResult>
	{
		public Guid CompanyUid { get; set; }

		public Guid UserUid { get; set; }

		public Event Item { get; set; }
	}
}
