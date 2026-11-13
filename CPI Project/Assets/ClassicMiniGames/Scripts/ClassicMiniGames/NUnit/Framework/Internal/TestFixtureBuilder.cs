using NUnit.Framework.Builders;
using NUnit.Framework.Extensibility;
using System;

namespace NUnit.Framework.Internal
{
	public class TestFixtureBuilder
	{
		private static ISuiteBuilder builder = new NUnitTestFixtureBuilder();

		public static bool CanBuildFrom(Type type)
		{
			return builder.CanBuildFrom(type);
		}

		public static Test BuildFrom(Type type)
		{
			return builder.BuildFrom(type);
		}

		public static Test BuildFrom(object fixture)
		{
			Test test = BuildFrom(fixture.GetType());
			if (test != null)
			{
				test.Fixture = fixture;
			}
			return test;
		}

		private TestFixtureBuilder()
		{
		}
	}
}
