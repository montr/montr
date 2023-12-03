using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Montr.Core.Models;
using Montr.Core.Queries;
using Montr.Core.Services;
using Montr.Core.Services.Implementations;
using Montr.Core.Services.QueryHandlers;
using Moq;
using NUnit.Framework;

namespace Montr.Core.Tests.QueryHandlers
{
	[TestFixture]
	public class GetMenuHandlerTests
	{
		[Test]
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
			Assert.That(result1?.Items, Is.Not.Null);
			Assert.That(result1.Id, Is.EqualTo("m1"));
			Assert.That(result1.Items.Count, Is.EqualTo(5));
			Assert.That(result1.Items[0].Id, Is.EqualTo("m1.1"));
			Assert.That(result1.Items[1].Id, Is.EqualTo("m1.2"));
			Assert.That(result1.Items[2].Id, Is.EqualTo("m1.3"));
			Assert.That(result1.Items[3].Id, Is.EqualTo("m1.4"));
			Assert.That(result1.Items[4].Id, Is.EqualTo("m1.5"));
		}

		[Test]
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
			Assert.That(result1?.Items, Is.Not.Null);
			Assert.That(result1.Id, Is.EqualTo("m1"));
			Assert.That(result1.Items.Count, Is.EqualTo(2));
			Assert.That(result1.Items[0].Id, Is.EqualTo("m1.1"));
			Assert.That(result1.Items[1].Id, Is.EqualTo("m1.3"));
		}
	}
}
