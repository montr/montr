using System;
using Kompany.Entities;
using Kompany.Models;
using LinqToDB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Montr.Data.Linq2Db;

namespace Kompany.Controllers
{
	[Authorize, ApiController, Route("api/[controller]/[action]")]
	public class CompanyController : ControllerBase
	{
		[HttpPost]
		public ActionResult<Guid> Create(Company item)
		{
			using (var db = new DbContext())
			{
				// var id = db.Execute<long>("select nextval('company_id_seq')");

				var uid = Guid.NewGuid();

				db.GetTable<DbCompany>()
					.Value(x => x.Uid, uid)
					.Value(x => x.ConfigCode, item.ConfigCode ?? "business")
					.Value(x => x.StatusCode, CompanyStatusCode.Draft)
					.Value(x => x.Name, item.Name)
					.Insert();

				return uid;
			}
		}
	}
}
