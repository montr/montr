﻿using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Montr.Core.Models;

namespace Montr.Core.Services
{
	public static class QueryableExtensions
	{
		// todo: pass viewId instead of defaultSortColumn (?)
		public static IQueryable<T> Apply<T>(this IQueryable<T> source, Paging paging,
			Expression<Func<T, object>> defaultSortColumn, SortOrder defaultSortOrder = SortOrder.Ascending)
		{
			if (paging == null)
			{
				paging = new Paging();
			}

			if (paging.SortColumn == null)
			{
				paging.SortColumn = ExpressionHelper.GetMemberName(defaultSortColumn);
			}

			if (paging.SortOrder == null)
			{
				paging.SortOrder = defaultSortOrder;
			}

			// todo: add skip order?
			var ordered = paging.SortOrder == SortOrder.Ascending
				? source.OrderBy(paging.SortColumn)
				: source.OrderByDescending(paging.SortColumn);

			if (paging.SkipPaging)
			{
				// todo: always should be set limit
				return ordered;
			}

			if (paging.PageNo <= 0)
			{
				paging.PageNo = 1;
			}

			if (paging.PageSize <= 0 || paging.PageSize > Paging.MaxPageSize)
			{
				paging.PageSize = Paging.DefaultPageSize;
			}

			var paged = ordered
				.Skip((paging.PageNo - 1) * paging.PageSize)
				.Take(paging.PageSize);

			return paged;
		}

		public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string property)
		{
			return ApplyOrder(source, property, nameof(Queryable.OrderBy));
		}

		public static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> source, string property)
		{
			return ApplyOrder(source, property, nameof(Queryable.OrderByDescending));
		}

		public static IOrderedQueryable<T> ThenBy<T>(this IOrderedQueryable<T> source, string property)
		{
			return ApplyOrder(source, property, nameof(Queryable.ThenBy));
		}

		public static IOrderedQueryable<T> ThenByDescending<T>(this IOrderedQueryable<T> source, string property)
		{
			return ApplyOrder(source, property, nameof(Queryable.ThenByDescending));
		}

		private static IOrderedQueryable<T> ApplyOrder<T>(IQueryable<T> source, string property, string methodName)
		{
			var type = typeof(T);

			var arg = Expression.Parameter(type, "x");
			Expression expr = arg;

			var props = property.Split('.');
			foreach (var prop in props)
			{
				// use reflection (not ComponentModel) to mirror LINQ
				var pi = type.GetProperty(prop,
					BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

				if (pi == null) throw new InvalidOperationException($"Property with name \"{prop}\" not found.");

				expr = Expression.Property(expr, pi);
				type = pi.PropertyType;
			}

			var delegateType = typeof(Func<,>).MakeGenericType(typeof(T), type);
			var lambda = Expression.Lambda(delegateType, expr, arg);

			var result = typeof(Queryable).GetMethods().Single(
					method => method.Name == methodName
							&& method.IsGenericMethodDefinition
							&& method.GetGenericArguments().Length == 2
							&& method.GetParameters().Length == 2)
				.MakeGenericMethod(typeof(T), type)
				.Invoke(null, new object[] { source, lambda });

			return (IOrderedQueryable<T>)result;
		}

		public static long? GetTotalCount<T>(this IQueryable<T> source, Paging paging)
		{
			if (source == null || paging == null || paging.SkipPaging)
			{
				return null;
			}

			return source.LongCount();
		}
	}
}
