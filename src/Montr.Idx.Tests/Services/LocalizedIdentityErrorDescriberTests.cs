using System;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using NUnit.Framework;

namespace Montr.Idx.Tests.Services
{
	[TestFixture]
	public class LocalizedIdentityErrorDescriberTests
	{
		[Test]
		public void ExtractResources()
		{
			// https://github.com/aspnet/AspNetIdentity/blob/master/src/Microsoft.AspNet.Identity.Core/Resources.Designer.cs
			var methods = typeof(IdentityErrorDescriber).GetMethods().Where(x => x.ReturnType == typeof(IdentityError));

			var instance = new IdentityErrorDescriber();

			foreach (var method in methods)
			{
				var parameters = method.GetParameters();

				var args = parameters.Length == 1
					? parameters[0].ParameterType == typeof(int) ? new object[] { 0 }
					: parameters[0].ParameterType == typeof(string) ? new object[] { "{0}" }
					: null : null;

				var result = (IdentityError)method.Invoke(instance, args);

				Console.WriteLine($"\"{nameof(IdentityError)}.{result.Code}\": \"{result.Description}\",");
			}
		}
	}
}
