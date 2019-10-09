using System.IO;

namespace Montr.Core.Models
{
	public class FileResult
	{
		public string ContentType { get; set; }

		public string FileName { get; set; }

		public Stream Stream { get; set; }
	}
}
