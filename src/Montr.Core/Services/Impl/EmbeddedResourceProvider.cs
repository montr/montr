using System;
using System.IO;
using System.Threading.Tasks;

namespace Montr.Core.Services.Impl
{
	public class EmbeddedResourceProvider
	{
		public Stream GetEmbeddedResourceStream(Type type, string name)
		{
			return type.Assembly.GetManifestResourceStream(type, name);
		}

		public async Task<string> LoadEmbeddedResource(Type type, string name)
		{
			using (var stream = GetEmbeddedResourceStream(type, name))
			{
				using (var reader = new StreamReader(stream ?? throw new ApplicationException($"Resource \"{name}\" is not found in {type.Assembly}.")))
				{
					return await reader.ReadToEndAsync();
				}
			}
		}
	}
}
