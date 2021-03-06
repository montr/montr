﻿using System;
using MediatR;
using Montr.MasterData.Models;

namespace Montr.MasterData.Queries
{
	public class GetClassifierGroup : IRequest<ClassifierGroup>
	{
		public string TypeCode { get; set; }

		public Guid TreeUid { get; set; }

		public Guid Uid { get; set; }
	}
}
