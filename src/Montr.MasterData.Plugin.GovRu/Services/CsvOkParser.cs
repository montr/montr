using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LumenWorks.Framework.IO.Csv;
using Montr.MasterData.Plugin.GovRu.Models;

namespace Montr.MasterData.Plugin.GovRu.Services
{
	public abstract class CsvOkParser<TItem> : OkParser<TItem> where TItem : OkItem, new()
	{
		private readonly List<Column> _columns = new List<Column>();

		protected char Delimiter = ';';
		protected char Quote = '"';
		protected char Escape = '\\';
		protected char Comment = '#';

		protected void Map(Expression<Func<TItem, object>> column, string pattern = null)
		{
			_columns.Add(new Column { Info = GetPropertyInfo(column), Pattern = pattern });
		}

		public override Task Parse(Stream stream, CancellationToken cancellationToken)
		{
			using (var reader = new CsvReader(stream, false, Encoding.UTF8, Delimiter, Quote, Escape, Comment, ValueTrimmingOptions.All))
			{
				reader.ParseError += (sender, args) =>
				{
					// Console.WriteLine(args.Error.Message);
					// args.Action = ParseErrorAction.AdvanceToNextLine;
				};

				foreach (var record in reader)
				{
					var item = Parse(record);

					_items.Add(item);
				}
			}

			return Task.CompletedTask;
		}

		protected virtual TItem Parse(string[] record)
		{
			var item = new TItem();

			for (var i = 0; i < Math.Min(record.Length, _columns.Count); i++)
			{
				if (record[i] != null)
				{
					var value = Convert(record[i], _columns[i].Info.PropertyType);

					_columns[i].Info.SetValue(item, value);
				}
			}
			
			return item;
		}

		private static PropertyInfo GetPropertyInfo(Expression expression)
		{
			var lambda = expression as LambdaExpression;

			if (lambda == null) throw new ArgumentNullException(nameof(expression));

			MemberExpression memberExpression = null;

			if (lambda.Body.NodeType == ExpressionType.Convert)
			{
				memberExpression = ((UnaryExpression)lambda.Body).Operand as MemberExpression;
			}
			else if (lambda.Body.NodeType == ExpressionType.MemberAccess)
			{
				memberExpression = lambda.Body as MemberExpression;
			}
			
			if (memberExpression == null)
			{
				throw new ArgumentException(nameof(expression));
			}
			
			return (PropertyInfo)memberExpression.Member;
		}

		private class Column
		{
			public PropertyInfo Info { get; set; }

			// public Expression<Func<TItem, object>> Expr { get; set; }

			public string Pattern { get; set; }
		}
	}
}
