using DisneyMobile.CoreUnitySystems.Utility;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace DisneyMobile.CoreUnitySystems.Test
{
	[TestFixture]
	public class ConfigurableSystem_Test
	{
		private class ConfigurableSystem : IConfigurable
		{
			public string mTestValue1 = "NeverSet";

			public double mTestValue2 = 0.0;

			public int mTestValue3 = 0;

			public bool mConfigureCalled = false;

			public bool mReconfigureCalled = false;

			public void Configure(IDictionary<string, object> dictionary)
			{
				mConfigureCalled = true;
				mTestValue1 = (dictionary["values"].AsDic()["mTestValue1"] as string);
				mTestValue2 = (double)dictionary["values"].AsDic()["mTestValue2"];
				mTestValue3 = (int)dictionary["values"].AsDic()["mTestValue3"];
			}

			public void Reconfigure(IDictionary<string, object> dictionary)
			{
				mReconfigureCalled = true;
				Configure(dictionary);
			}
		}

		private string mLowestPriorityConfigFile;

		private Configurator _configurator = null;

		private ConfigurableSystem _configurableSystem = null;

		[SetUp]
		public void SetUp()
		{
			string text = Application.dataPath + "/Core/GameSystems/Configurator/UnitTest/";
			mLowestPriorityConfigFile = text + "ApplicationConfig.txt";
			_configurator = new Configurator();
			_configurator.SetConfigurationPath(text);
			_configurator.Init(true);
			_configurableSystem = new ConfigurableSystem();
		}

		[TearDown]
		public void TearDown()
		{
			FileHelper.DeleteIfExists(mLowestPriorityConfigFile);
			_configurator = null;
			_configurableSystem = null;
		}

		[Test]
		public void TestCreation()
		{
			Assert.NotNull(_configurator);
			Assert.NotNull(_configurableSystem);
		}

		[Test]
		public void TestConfiguration()
		{
			IDictionary<string, object> dictionaryForSystem = _configurator.GetDictionaryForSystem(typeof(ConfigurableSystem));
			_configurableSystem.Configure(dictionaryForSystem);
			Assert.That(_configurableSystem.mConfigureCalled, Is.EqualTo(true));
			Assert.That(_configurableSystem.mTestValue1, Is.EqualTo("HasBeenSet"));
			Assert.That(_configurableSystem.mTestValue2, Is.EqualTo(3.14159));
			Assert.That(_configurableSystem.mTestValue3, Is.EqualTo(42));
		}

		[Test]
		public void TestReconfiguration()
		{
			IDictionary<string, object> dictionaryForSystem = _configurator.GetDictionaryForSystem(typeof(ConfigurableSystem));
			_configurableSystem.mTestValue1 = "SomethingElse";
			_configurableSystem.mTestValue2 = 1.0;
			_configurableSystem.mTestValue3 = 4096;
			_configurableSystem.Reconfigure(dictionaryForSystem);
			Assert.That(_configurableSystem.mReconfigureCalled, Is.EqualTo(true));
			Assert.That(_configurableSystem.mTestValue1, Is.EqualTo("HasBeenSet"));
			Assert.That(_configurableSystem.mTestValue2, Is.EqualTo(3.14159));
			Assert.That(_configurableSystem.mTestValue3, Is.EqualTo(42));
		}
	}
}
