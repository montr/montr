using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.MasterData.Models;
using Montr.MasterData.Queries;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Montr.MasterData.Services.QueryHandlers
{
	public class ExportClassifierListHandler : IRequestHandler<ExportClassifierList, FileResult>
	{
		private static readonly int ColumnNameRow = 1;
		private static readonly int ColumnCodeRow = 2;

		private static readonly int FirstDataRow = 3;
		private static readonly int FirstDataCol = 1;

		private readonly INamedServiceFactory<IClassifierRepository> _repositoryFactory;

		public ExportClassifierListHandler(INamedServiceFactory<IClassifierRepository> repositoryFactory)
		{
			_repositoryFactory = repositoryFactory;
		}

		public async Task<FileResult> Handle(ExportClassifierList command, CancellationToken cancellationToken)
		{
			command.PageNo = 1;
			command.PageSize = Paging.MaxPageSize;

			var repository = _repositoryFactory.GetNamedOrDefaultService(command.TypeCode);

			var data = await repository.Search(command, cancellationToken);

			using (var package = new ExcelPackage())
			{
				var ws = package.Workbook.Worksheets.Add(command.TypeCode ?? nameof(Classifier));

				// header
				var col = FirstDataCol;

				var columns = new[]
				{
					new { Code = "uid", Name = "" },
					new { Code = "code", Name = "Код" },
					new { Code = "name", Name = "Наименование" },
					new { Code = "statusCode", Name = "Статус" }
				};

				foreach (var column in columns)
				{
					ws.Cells[ColumnNameRow, col].Value = column.Name;
					ws.Cells[ColumnCodeRow, col].Value = column.Code;

					col++;
				}

				StyleHeader(command, ws);

				// data
				var row = FirstDataRow;

				foreach (var entity in data.Rows)
				{
					col = FirstDataCol;

					var entityType = entity.GetType();

					foreach (var column in columns)
					{
						var cell = ws.Cells[row, col];

						var property = entityType.GetProperty(column.Code, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
						var value = property?.GetMethod?.Invoke(entity, null);

						/*string format;
						if (IsExcelSupportedType(column, out format))
						{
							cell.Style.Numberformat.Format = format;
							cell.Value = value;
						}
						else*/
						{
							// var typeConverter = _typeConverterFactory(column.DataType);

							cell.Value = value; // typeConverter.ConvertToInvariantString(column, value);
						}

						col++;
					}

					row++;
				}

				StyleData(command, ws);

				return new FileResult
				{
					ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
					FileName = $"{command.TypeCode ?? nameof(Classifier)}-{DateTime.Now.ToString("u").Replace(':', '-').Replace(' ', '-')}.xlsx",
					Stream = new MemoryStream(package.GetAsByteArray())
				};
			}
		}

		private static void StyleHeader(ExportClassifierList command, ExcelWorksheet ws)
		{
			var row1 = ws.Cells[1, 1, 1, ws.Dimension.Columns];
			var row2 = ws.Cells[2, 1, 2, ws.Dimension.Columns];

			if (command.AutoFitColumns)
			{
				row1.AutoFitColumns(8, 16);
			}

			row1.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
			row1.Style.WrapText = true;
			row1.Style.Font.Bold = true;
			row1.Style.Font.Size = 10;

			row2.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
			row2.Style.Font.Color.SetColor(Color.Gray);
			row2.Style.Font.Size = 8;
		}

		private static void StyleData(ExportClassifierList command, ExcelWorksheet ws)
		{
			var col1 = ws.Cells[1, 1, ws.Dimension.Rows, 1];
			col1.Style.Font.Color.SetColor(Color.Gray);
			col1.Style.Font.Size = 8;

			ws.Column(1).Width = 0; // Id

			if (command.AutoFitColumns)
			{
				ws.Column(2).AutoFit(12, 24); // Code
				ws.Column(3).AutoFit(12, 96); // Name
			}

			ws.View.FreezePanes(3, 3);
		}
	}
}
