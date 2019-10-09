using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Montr.Core.Controllers
{
	[/*Authorize,*/ ApiController, Route("api/[controller]/[action]")]
	public class LocaleController
	{
		private readonly IDictionary<string, IDictionary<string, string>> _langs =
			new Dictionary<string, IDictionary<string, string>>
			{
				{
					"en", new Dictionary<string, string>
					{
						{"confirm.title", "Confirm operation"},
						{"publish.confirm.content", "Are you sure you want to publish the event?"}
					}
				},
				{
					"ru", new Dictionary<string, string>
					{
						{"confirm.title", "Подтверждение операции"},
						{"publish.confirm.content", "Вы действительно хотите опубликовать событие?"}
					}
				},
			};


		[HttpGet, Route("{lng}/{ns}")]
		public Task<IDictionary<string, string>> List([FromRoute]string lng, [FromRoute]string ns)
		{
			return Task.FromResult(_langs[lng]);
		}
	}
}
