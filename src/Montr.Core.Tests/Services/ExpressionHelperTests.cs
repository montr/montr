using Montr.Core.Services;
using NUnit.Framework;

namespace Montr.Core.Tests.Services
{
	[TestFixture]
	public class ExpressionHelperTests
	{
		[Test]
		public void GetMemberName_Should_ReturnExpressionAsString()
		{
			var result = ExpressionHelper.GetFullName<Resources>(x => x.Key1);

			Assert.That(result, Is.EqualTo("Montr.Core.Tests.Services.ExpressionHelperTests+Resources.Key1"));
		}

		private abstract class Resources
		{
			public abstract string Key1 { get; }
		}
	}
}
