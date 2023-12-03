using System.Linq;
using Montr.Core.Models;
using Montr.Core.Services.Implementations;
using NUnit.Framework;

namespace Montr.Core.Tests.Services
{
	[TestFixture]
	public class DefaultConfigurationManagerTests
	{
		[Test]
		public void GetItems_ForTestItem_ShouldReturnList()
		{
			// arrange
			var registry = new DefaultConfigurationRegistry();

			registry.Configure<Person>(config =>
			{
				config.When(x => x.StatusCode == "draft")
					.Add<Block>((_, block) => block.Url = "/do");

				config.When(x => x.StatusCode != "draft")
					.Add<InfoItem>()
					.Add<Block>((_, block) => block.Url = "/done")
					.Add<HistoryBlock>((entity, block) => block.Url = "/to?status=" + entity.StatusCode);
			});

			registry.Configure<Entrepreneur>(config =>
			{
				config.Add<InfoItem>();
			});

			var userDraft = new Person { StatusCode = "draft"};
			var userBlocked = new Person { StatusCode = "inactive"};
			var company = new Entrepreneur();

			// act
			var userDraftItems = registry.GetItems<Person, InfoItem>(userDraft).ToList();
			var userDraftBlocks = registry.GetItems<Person, Block>(userDraft).ToList();

			var userBlockedItems = registry.GetItems<Person, InfoItem>(userBlocked).ToList();
			var userBlockedBlocks = registry.GetItems<Person, Block>(userBlocked).ToList();

			var companyItems = registry.GetItems<Entrepreneur, InfoItem>(company).ToList();
			var companyBlocks = registry.GetItems<Entrepreneur, Block>(company).ToList();

			// assert
			Assert.That(userDraftItems.Count, Is.EqualTo(0));
			Assert.That(userDraftBlocks.Count, Is.EqualTo(1));

			Assert.That(userBlockedItems.Count, Is.EqualTo(1));
			Assert.That(userBlockedItems[0].GetType(), Is.EqualTo(typeof(InfoItem)));

			Assert.That(userBlockedBlocks.Count, Is.EqualTo(2));
			Assert.That(userBlockedBlocks[0].GetType(), Is.EqualTo(typeof(Block)));
			Assert.That(userBlockedBlocks[0].Url, Is.EqualTo("/done"));
			Assert.That(userBlockedBlocks[1].GetType(), Is.EqualTo(typeof(HistoryBlock)));
			Assert.That(userBlockedBlocks[1].Url, Is.EqualTo("/to?status=inactive"));

			Assert.That(companyItems.Count, Is.EqualTo(1));
			Assert.That(companyBlocks.Count, Is.EqualTo(0));
		}

		private class Person
		{
			public string StatusCode { get; init; }
		}

		private class Entrepreneur
		{
		}

		private class InfoItem : ConfigurationItem
		{
		}

		private class Block : ConfigurationItem
		{
			public string Url { get; set; }
		}

		private class HistoryBlock : Block
		{
		}
	}
}
