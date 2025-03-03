using NUnit.Framework;
using System.Collections.Generic;

namespace DisneyMobile.CoreUnitySystems.Test
{
	[TestFixture]
	public class AutoConfigurable_Test
	{
		private class ConfigurableSystem : AutoConfigurable
		{
			private string mTestValue1 = "NeverSet";

			private float mTestValue2 = 0f;

			private int mTestValue3 = 0;

			private bool mConfigured = false;

			private bool mReconfigured = false;

			public string GetTestValue1()
			{
				return mTestValue1;
			}

			public float GetTestValue2()
			{
				return mTestValue2;
			}

			public int GetTestValue3()
			{
				return mTestValue3;
			}

			public bool IsConfigured()
			{
				return mConfigured;
			}

			public bool IsReconfigured()
			{
				return mReconfigured;
			}

			public override void Configure(IDictionary<string, object> dictionary)
			{
				base.Configure(dictionary);
				mConfigured = true;
			}

			public override void Reconfigure(IDictionary<string, object> dictionary)
			{
				base.Reconfigure(dictionary);
				mReconfigured = true;
			}
		}

		private ConfigurableSystem _system = null;

		[SetUp]
		public void SetUp()
		{
			_system = new ConfigurableSystem();
		}

		[TearDown]
		public void TearDown()
		{
			_system = null;
		}

		[Test]
		public void TestCreation()
		{
			Assert.NotNull(_system);
		}

		[Test]
		public void TestConfigure()
		{
			IDictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["values"] = new Dictionary<string, object>();
			dictionary["values"].AsDic()["mTestValue1"] = "SetInConfiguration";
			dictionary["values"].AsDic()["mTestValue2"] = 3.14159f;
			dictionary["values"].AsDic()["mTestValue3"] = 42;
			_system.Configure(dictionary);
			Assert.That(_system.IsConfigured(), Is.EqualTo(true));
			Assert.That(_system.GetTestValue1(), Is.EqualTo("SetInConfiguration"));
			Assert.That(_system.GetTestValue2(), Is.EqualTo(3.14159f));
			Assert.That(_system.GetTestValue3(), Is.EqualTo(42));
		}

		[Test]
		public void TestReconfigure()
		{
			IDictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["values"] = new Dictionary<string, object>();
			dictionary["values"].AsDic()["mTestValue1"] = "SetInReconfiguration";
			dictionary["values"].AsDic()["mTestValue2"] = 2.71828f;
			dictionary["values"].AsDic()["mTestValue3"] = 64;
			_system.Reconfigure(dictionary);
			Assert.That(_system.IsReconfigured(), Is.EqualTo(true));
			Assert.That(_system.GetTestValue1(), Is.EqualTo("SetInReconfiguration"));
			Assert.That(_system.GetTestValue2(), Is.EqualTo(2.71828f));
			Assert.That(_system.GetTestValue3(), Is.EqualTo(64));
		}
	}
}
