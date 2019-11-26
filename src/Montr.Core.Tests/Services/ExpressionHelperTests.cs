using Microsoft.VisualStudio.TestTools.UnitTesting;
using Montr.Core.Services;

namespace Montr.Core.Tests.Services
{
	[TestClass]
	public class ExpressionHelperTests
	{
		[TestMethod]
		public void GetMemberName_Should_ReturnExpressionAsString()
		{
			var result = ExpressionHelper.GetFullName<Resources>(x => x.Key1);

			Assert.AreEqual("Montr.Core.Tests.Services.ExpressionHelperTests+Resources.Key1", result);
		}

		private abstract class Resources
		{
			public abstract string Key1 { get; }
		}
	}
}
