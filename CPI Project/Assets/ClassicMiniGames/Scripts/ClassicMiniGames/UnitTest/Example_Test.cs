using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;

namespace UnitTest
{
	[TestFixture]
	public class Example_Test
	{
		private int[] source01;

		private List<int> source02;

		[SetUp]
		public void InitSources()
		{
			source01 = new int[3]
			{
				1,
				2,
				3
			};
			source02 = new List<int>(source01);
		}

		[TearDown]
		public void DestroySources()
		{
			source01 = null;
			source02 = null;
		}

		[Test]
		public void Add()
		{
			int num = source01.Length + source02.Count;
			Assert.That(num, Is.EqualTo(6));
		}

		[ExpectedException(typeof(InvalidCastException))]
		[Test]
		public void ExpectAnException()
		{
			throw new InvalidCastException();
		}

		[Test]
		public void SourcesAreEqual()
		{
			Assert.That(source02, Is.EquivalentTo(source01));
			Assert.That(source01, Is.EqualTo(source02));
			Assert.That(source01, Is.EqualTo(source02).AsCollection);
		}

		[Test]
		public void SourcesAreSubsetOfEachOther()
		{
			Assert.That(source01, Is.SubsetOf(source02));
			Assert.That(source02, Is.SubsetOf(source01));
		}

		[Test]
		public void AllSource01MembersLessThen5()
		{
			Assert.That(source01, Is.All.LessThan(5) & Is.Unique);
			Assert.That(source01, Is.All.InRange(1, 3));
			Assert.That(source02, Is.All.InRange(1, 3).Using(Comparer.Default));
		}
	}
}
