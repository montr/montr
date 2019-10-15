using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Models;

namespace Montr.Core.Impl.Services
{
	// todo: use Montr.Core.Services.IJsonSerializer
	public class LocaleStringSerializer
	{
		public async Task<Stream> Serialize(LocaleStringList[] data, CancellationToken cancellationToken)
		{
			var value = data.ToDictionary(x => x.Locale,
				locale => locale.Modules.ToDictionary(x => x.Module,
					module => module.Items.ToDictionary(x => x.Key, x => x.Value)));
			
			var stream = new MemoryStream();

			var options = new JsonSerializerOptions
			{
				WriteIndented = true,
				Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
			};

			await JsonSerializer.SerializeAsync(stream, value, options, cancellationToken);

			stream.Position = 0;

			return stream;
		}

		public async Task<IList<LocaleStringList>> Deserialize(Stream json, CancellationToken cancellationToken)
		{
			var value = await JsonSerializer
				.DeserializeAsync<Dictionary<string, Dictionary<string, Dictionary<string, string>>>>(json, null, cancellationToken);

			var result = new List<LocaleStringList>();

			foreach (var (locale, modules) in value)
			{
				var item = new LocaleStringList
				{
					Locale = locale,
					Modules = new List<LocaleModuleStringList>()
				};

				foreach (var (module, items) in modules)
				{
					item.Modules.Add(new LocaleModuleStringList
					{
						Module = module,
						Items = items.Select(x => new LocaleString { Key = x.Key, Value = x.Value } ).ToList()
					});
				}

				result.Add(item);
			}

			return result;
		}
	}
}
