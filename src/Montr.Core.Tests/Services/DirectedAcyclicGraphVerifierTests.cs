using System;
using Montr.Core.Services.Implementations;
using NUnit.Framework;

namespace Montr.Core.Tests.Services
{
	[TestFixture]
	public class DirectedAcyclicGraphVerifierTests
	{
		[Test]
		public void TopologicalSort_WithoutCycles1_ShouldSort()
		{
			// arrange
			var a = new Node { Name = "A" };
			var b = new Node { Name = "B" };
			var c = new Node { Name = "C" };
			var d = new Node { Name = "D" };
			var e = new Node { Name = "E" };

			a.DependsOn = new[] { "B", "D" };
			b.DependsOn = new[] { "C", "E" };

			// act
			var result = DirectedAcyclicGraphVerifier.TopologicalSort(
				new[] { a, b, c, d, e }, node => node.Name, node => node.DependsOn);

			// assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Count, Is.EqualTo(5));
			Assert.That(result[0].Name, Is.EqualTo("C"));
			Assert.That(result[1].Name, Is.EqualTo("E"));
			Assert.That(result[2].Name, Is.EqualTo("B"));
			Assert.That(result[3].Name, Is.EqualTo("D"));
			Assert.That(result[4].Name, Is.EqualTo("A"));
		}

		[Test]
		public void TopologicalSort_WithoutCycles2_ShouldSort()
		{
			// arrange
			var a = new Node { Name = "A" };
			var b = new Node { Name = "B" };
			var c = new Node { Name = "C" };
			var d = new Node { Name = "D" };

			a.DependsOn = new[] { "B", "C" };
			b.DependsOn = new[] { "D" };
			c.DependsOn = new[] { "D" };

			// act
			var result = DirectedAcyclicGraphVerifier.TopologicalSort(
				new[] { a, b, c, d }, node => node.Name, node => node.DependsOn);

			// assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Count, Is.EqualTo(4));
			Assert.That(result[0].Name, Is.EqualTo("D"));
			Assert.That(result[1].Name, Is.EqualTo("B"));
			Assert.That(result[2].Name, Is.EqualTo("C"));
			Assert.That(result[3].Name, Is.EqualTo("A"));
		}

		[Test]
		public void TopologicalSort_WithoutCycles3_ShouldSort()
		{
			// arrange
			var a = new Node { Name = "A" };
			var b = new Node { Name = "B" };
			var c = new Node { Name = "C" };
			var d = new Node { Name = "D" };

			// act
			var result = DirectedAcyclicGraphVerifier.TopologicalSort(
				new[] { a, b, c, d }, node => node.Name, node => node.DependsOn);

			// assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Count, Is.EqualTo(4));
			Assert.That(result[0].Name, Is.EqualTo("A"));
			Assert.That(result[1].Name, Is.EqualTo("B"));
			Assert.That(result[2].Name, Is.EqualTo("C"));
			Assert.That(result[3].Name, Is.EqualTo("D"));
		}

		[Test]
		public void TopologicalSort_WithCycles1_ShouldThrow()
		{
			// arrange
			var a = new Node { Name = "A" };
			var b = new Node { Name = "B" };
			var c = new Node { Name = "C" };

			a.DependsOn = new[] { "B" };
			b.DependsOn = new[] { "C" };
			c.DependsOn = new[] { "A" };

			// act && assert
			Assert.Throws<InvalidOperationException>(
				() => DirectedAcyclicGraphVerifier.TopologicalSort(
					new[] { a, b, c }, node => node.Name, node => node.DependsOn));
		}


		[Test]
		public void TopologicalSort_WithCycles2_ShouldThrow()
		{
			// arrange
			var a = new Node { Name = "A" };
			var b = new Node { Name = "B" };
			var c = new Node { Name = "C" };
			var d = new Node { Name = "D" };

			a.DependsOn = new[] { "B" };
			b.DependsOn = new[] { "C" };
			c.DependsOn = new[] { "D" };
			a.DependsOn = new[] { "A" };

			// act && assert
			Assert.Throws<InvalidOperationException>(
				() => DirectedAcyclicGraphVerifier.TopologicalSort(
					new[] { a, b, c, d }, node => node.Name, node => node.DependsOn));
		}

		public class Node
		{
			public string Name { get; set; }

			public string[] DependsOn { get; set; }
		}
	}
}
