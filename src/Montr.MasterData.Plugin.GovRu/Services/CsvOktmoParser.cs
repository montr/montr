using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Montr.MasterData.Plugin.GovRu.Models;

namespace Montr.MasterData.Plugin.GovRu.Services
{
	public class CsvOktmoParser : CsvOkParser<OktmoItem>
	{
		public CsvOktmoParser()
		{
			Map(x => x.Code1);
			Map(x => x.Code2);
			Map(x => x.Code3);
			Map(x => x.Code4);
			Map(x => x.ControlNo);
			Map(x => x.SectionCode);
			Map(x => x.Name);
			Map(x => x.AdditionalInfo);
			Map(x => x.Description);
			Map(x => x.ActNo);
			Map(x => x.StatusNo);
			// Map(x => x.DateAccepted, "dd.MM.yyyy");
			// Map(x => x.StartDateActive, "dd.MM.yyyy");
		}

		// todo: make all not-imported items as inactive
		public override async Task Parse(Stream stream, CancellationToken cancellationToken)
		{
			var memoryStream = await FixStream(stream, CodePagesEncodingProvider.Instance.GetEncoding(1251));

			await base.Parse(memoryStream, cancellationToken);
		}

		private async Task<Stream> FixStream(Stream stream, Encoding encoding)
		{
			var stringBuilder = new StringBuilder();
			var memoryStream = new MemoryStream();

			using (var reader = new StreamReader(stream, encoding))
			{
				using (var writer = new StreamWriter(memoryStream, Encoding.UTF8, 4096, true))
				{
					string line;
					while ((line = await reader.ReadLineAsync()) != null)
					{
						var quoteStarted = false;
						var prevChar = '\0';

						foreach (var currChar in line)
						{
							if ((!quoteStarted && currChar == Quote && prevChar == ' ')
								|| (quoteStarted && currChar == Quote))
							{
								quoteStarted = !quoteStarted;
								stringBuilder.Append(Escape);
							}

							stringBuilder.Append(currChar);

							prevChar = currChar;
						}

						writer.WriteLine(stringBuilder.ToString());
						stringBuilder.Clear();
					}
				}
			}

			memoryStream.Position = 0;

			return memoryStream;
		}

		protected override OktmoItem Parse(string[] record)
		{
			var item = base.Parse(record);

			// todo: analyze parent/child relations
			if (item.SectionCode == "1")
			{
				item.Code = string.Concat(item.Code1, item.Code2, item.Code3);

				if (item.Code1 == "00" && item.Code2 == "000" && item.Code3 == "000")
				{
					// root
				}
				else if (item.Code2 == "000" && item.Code3 == "000")
				{
					item.ParentCode = string.Concat("00", "000", "000");
				}
				else if (item.Code2.EndsWith("00") == false && item.Code3 == "000")
				{
					item.ParentCode = string.Concat(item.Code1, item.Code2[0] + "00", "000");
				}
				else if (item.Code2 != "000" && item.Code3.EndsWith("00") == false)
				{
					item.ParentCode = string.Concat(item.Code1, item.Code2, item.Code3[0] + "00");
				}
				else if (item.Code3 != "000")
				{
					item.ParentCode = string.Concat(item.Code1, item.Code2, "000");
				}
				else if (item.Code2 != "000")
				{
					item.ParentCode = string.Concat(item.Code1, "000", "000");
				}
			}
			else
			{
				item.Code = string.Concat(item.Code1, item.Code2, item.Code3, item.Code4);

				if (item.Code1 == "00" && item.Code2 == "000" && item.Code3 == "000" && item.Code4 == "000")
				{
					// root?
				}
				else if (item.Code2 == "000" && item.Code3 == "000" && item.Code4 == "000")
				{
					item.ParentCode = string.Concat(item.Code1, item.Code2, item.Code3);
				}
				else if (item.Code2 != "000" && item.Code3 != "000" && item.Code4 == "000")
				{
					item.ParentCode = string.Concat(item.Code1, item.Code2, item.Code3);
				}
				else if (item.Code4 != "000")
				{
					item.ParentCode = string.Concat(item.Code1, item.Code2, item.Code3, "000");
				}
				else if (item.Code3 != "000")
				{
					item.ParentCode = string.Concat(item.Code1, item.Code2, "000", "000");
				}
				else if (item.Code2 != "000")
				{
					item.ParentCode = string.Concat(item.Code1, "000", "000", "000");
				}
			}

			return item;
		}
	}
}
