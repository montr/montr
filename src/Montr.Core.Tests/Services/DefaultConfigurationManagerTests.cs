using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Montr.Core.Impl.Services;
using Montr.Core.Models;

namespace Montr.Core.Tests.Services
{
	[TestClass]
	public class DefaultConfigurationManagerTests
	{
		[TestMethod]
		public void GetItems_ForTestItem_ShouldReturnList()
		{
			// arrange
			var manager = new DefaultConfigurationManager();

			manager.Configure<Person>(config =>
			{
				config.When(x => x.StatusCode == "draft")
					.Add<Block>((_, block) => block.Url = "/do");

				config.When(x => x.StatusCode != "draft")
					.Add<InfoItem>()
					.Add<Block>((_, block) => block.Url = "/done")
					.Add<HistoryBlock>((entity, block) => block.Url = "/to?status=" + entity.StatusCode);
			});

			manager.Configure<Entrepreneur>(config =>
			{
				config.Add<InfoItem>();
			});

			var userDraft = new Person { StatusCode = "draft"};
			var userBlocked = new Person { StatusCode = "inactive"};
			var company = new Entrepreneur();

			// act
			var userDraftItems = manager.GetItems<Person, InfoItem>(userDraft).ToList();
			var userDraftBlocks = manager.GetItems<Person, Block>(userDraft).ToList();

			var userBlockedItems = manager.GetItems<Person, InfoItem>(userBlocked).ToList();
			var userBlockedBlocks = manager.GetItems<Person, Block>(userBlocked).ToList();

			var companyItems = manager.GetItems<Entrepreneur, InfoItem>(company).ToList();
			var companyBlocks = manager.GetItems<Entrepreneur, Block>(company).ToList();

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
