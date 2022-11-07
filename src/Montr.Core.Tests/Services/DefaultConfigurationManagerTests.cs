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
			Assert.AreEqual(0, userDraftItems.Count);
			Assert.AreEqual(1, userDraftBlocks.Count);

			Assert.AreEqual(1, userBlockedItems.Count);
			Assert.AreEqual(typeof(InfoItem), userBlockedItems[0].GetType());

			Assert.AreEqual(2, userBlockedBlocks.Count);
			Assert.AreEqual(typeof(Block), userBlockedBlocks[0].GetType());
			Assert.AreEqual("/done", userBlockedBlocks[0].Url);
			Assert.AreEqual(typeof(HistoryBlock), userBlockedBlocks[1].GetType());
			Assert.AreEqual("/to?status=inactive", userBlockedBlocks[1].Url);

			Assert.AreEqual(1, companyItems.Count);
			Assert.AreEqual(0, companyBlocks.Count);
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
