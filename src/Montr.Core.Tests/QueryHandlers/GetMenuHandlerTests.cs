using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Montr.Core.Impl.QueryHandlers;
using Montr.Core.Impl.Services;
using Montr.Core.Models;
using Montr.Core.Queries;
using Montr.Core.Services;
using Moq;

namespace Montr.Core.Tests.QueryHandlers
{
	[TestClass]
	public class GetMenuHandlerTests
	{
		[TestMethod]
		public async Task GetMenu_ExistingId_ShouldReturnCombinedMenuItems()
		{
			// arrange
			var cancellationToken = CancellationToken.None;

			var authService = new Mock<IAuthorizationService>();

			var cp1 = new Mock<IContentProvider>();
			cp1.Setup(x => x.GetMenuItems("m1")).Returns(new[]
			{
				new Menu { Id = "m1.1" },
				new Menu { Id = "m1.2" },
				new Menu { Id = "m1.3" }
			});

			var cp2 = new Mock<IContentProvider>();
			cp2.Setup(x => x.GetMenuItems("m1")).Returns(new[]
			{
				new Menu { Id = "m1.4" },
				new Menu { Id = "m1.5" }
			});

			var service = new DefaultContentService(new [] { cp1.Object, cp2.Object });
			var handler = new GetMenuHandler(authService.Object, service);

			// act
			var result1 = await handler.Handle(new GetMenu { MenuId = "m1" }, cancellationToken);

			// assert
			Assert.IsNotNull(result1?.Items);
			Assert.AreEqual("m1", result1.Id);
			Assert.AreEqual(5, result1.Items.Count);
			Assert.AreEqual("m1.1", result1.Items[0].Id);
			Assert.AreEqual("m1.2", result1.Items[1].Id);
			Assert.AreEqual("m1.3", result1.Items[2].Id);
			Assert.AreEqual("m1.4", result1.Items[3].Id);
			Assert.AreEqual("m1.5", result1.Items[4].Id);
		}

		[TestMethod]
		public async Task GetMenu_WhenAllChildrenUnauthorized_ShouldNotReturnParent()
		{
			// arrange
			var cancellationToken = CancellationToken.None;

			var authService = new Mock<IAuthorizationService>();
			authService.Setup(x => x.AuthorizeAsync(
					It.IsAny<ClaimsPrincipal>(), null,
					It.Is<IEnumerable<IAuthorizationRequirement>>(
						mc => mc.Any(m => (m as PermissionRequirement).Permission == "denied"))))
				.Returns(Task.FromResult(AuthorizationResult.Failed()));

			var cp1 = new Mock<IContentProvider>();
			cp1.Setup(x => x.GetMenuItems("m1")).Returns(new[]
			{
				new Menu { Id = "m1.1" },
				new Menu { Id = "m1.2", Items = new []
				{
					new Menu { Id = "m1.2.1", Permission = "denied" },
					new Menu { Id = "m1.2.2", Permission = "denied" },
					new Menu { Id = "m1.2.3", Permission = "denied" }
				}},
				new Menu { Id = "m1.3" }
			});

			var service = new DefaultContentService(new [] { cp1.Object });
			var handler = new GetMenuHandler(authService.Object, service);

			// act
			var result1 = await handler.Handle(new GetMenu { MenuId = "m1" }, cancellationToken);

			// assert
			Assert.IsNotNull(result1?.Items);
			Assert.AreEqual("m1", result1.Id);
			Assert.AreEqual(2, result1.Items.Count);
			Assert.AreEqual("m1.1", result1.Items[0].Id);
			Assert.AreEqual("m1.3", result1.Items[1].Id);
		}
	}
}
