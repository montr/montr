using System;
using System.Collections.Generic;
using MediatR;
using Montr.Docs.Models;

namespace Montr.Kompany.Registration.Queries
{
	public class GetCompanyRegistrationRequestList :  IRequest<ICollection<Document>>
	{
		public Guid? UserUid { get; set; }
	}
}
