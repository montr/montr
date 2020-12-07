﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Montr.Core.Services;

namespace Montr.Core.Tests.Services
{
	[TestClass]
	public class DirectedAcyclicGraphVerifierTests
	{
		[TestMethod]
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
			Assert.IsNotNull(result);
			Assert.AreEqual(5, result.Count);
			Assert.AreEqual("C", result[0].Name);
			Assert.AreEqual("E", result[1].Name);
			Assert.AreEqual("B", result[2].Name);
			Assert.AreEqual("D", result[3].Name);
			Assert.AreEqual("A", result[4].Name);
		}

		[TestMethod]
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
			Assert.IsNotNull(result);
			Assert.AreEqual(4, result.Count);
			Assert.AreEqual("D", result[0].Name);
			Assert.AreEqual("B", result[1].Name);
			Assert.AreEqual("C", result[2].Name);
			Assert.AreEqual("A", result[3].Name);
		}

		[TestMethod]
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
			Assert.IsNotNull(result);
			Assert.AreEqual(4, result.Count);
			Assert.AreEqual("A", result[0].Name);
			Assert.AreEqual("B", result[1].Name);
			Assert.AreEqual("C", result[2].Name);
			Assert.AreEqual("D", result[3].Name);
		}

		[TestMethod]
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
			Assert.ThrowsException<InvalidOperationException>(
				() => DirectedAcyclicGraphVerifier.TopologicalSort(
					new[] { a, b, c }, node => node.Name, node => node.DependsOn));
		}


		[TestMethod]
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
			Assert.ThrowsException<InvalidOperationException>(
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
