using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Montr.Core.Services;

namespace Montr.Core.Tests.Services
{
	[TestClass]
	public class DirectedAcyclicGraphVerifierTests
	{
		[TestMethod]
		public void TopologicalSort_WithoutCicles_ShouldSort()
		{
			// arrange
			var a = new Node { Name = "A" };
			var b = new Node { Name = "B" };
			var c = new Node { Name = "C" };
			var d = new Node { Name = "D" };
			var e = new Node { Name = "E" };

			a.DependsOn = new[] { b, d };
			b.DependsOn = new[] { c, e };

			// act
			var result = DirectedAcyclicGraphVerifier.Sort(
				new[] { a, b, c, d, e },
				node => node.Name, node => node.DependsOn?.Select(x => x.Name));

			// assert
			Assert.IsNotNull(result);
			Assert.AreEqual(5, result.Count);
			Assert.AreEqual("C", result[0].Name);
			Assert.AreEqual("E", result[1].Name);
			Assert.AreEqual("B", result[2].Name);
			Assert.AreEqual("D", result[3].Name);
			Assert.AreEqual("A", result[4].Name);
		}

		[TestMethod]
		public void TopologicalSort_WithCicles_ShouldThrow()
		{
			// arrange
			var a = new Node { Name = "A" };
			var b = new Node { Name = "B" };
			var c = new Node { Name = "C" };

			a.DependsOn = new[] { b };
			b.DependsOn = new[] { c };
			c.DependsOn = new[] { a };

			// act && assert
			Assert.ThrowsException<InvalidOperationException>(
				() => DirectedAcyclicGraphVerifier.Sort(
					new[] { a, b, c },
					node => node.Name, node => node.DependsOn.Select(x => x.Name)));
		}

		public class Node
		{
			public string Name { get; set; }

			public Node[] DependsOn { get; set; }
		}
	}
}
