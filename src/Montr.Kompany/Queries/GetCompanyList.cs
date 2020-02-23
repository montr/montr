using System;
using System.Collections.Generic;
using MediatR;
using Montr.Kompany.Models;

namespace Montr.Kompany.Queries
{
	public class GetCompanyList : IRequest<IList<Company>>
	{
		public Guid UserUid { get; set; }
	}
}
