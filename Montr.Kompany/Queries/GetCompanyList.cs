using System;
using System.Collections.Generic;
using Kompany.Models;
using MediatR;

namespace Kompany.Queries
{
	public class GetCompanyList : IRequest<IList<Company>>
	{
		public Guid UserUid { get; set; }
	}
}
