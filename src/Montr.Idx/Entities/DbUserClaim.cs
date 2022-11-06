using System;
using LinqToDB.Identity;
using LinqToDB.Mapping;

namespace Montr.Idx.Entities
{
	[Table(Schema = "montr", Name = "user_claim")]
	public class DbUserClaim : IdentityUserClaim<Guid>
	{
	}
}
