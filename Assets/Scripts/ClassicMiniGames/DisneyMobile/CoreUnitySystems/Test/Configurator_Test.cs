using DisneyMobile.CoreUnitySystems.Utility;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace DisneyMobile.CoreUnitySystems.Test
{
	[TestFixture]
	public class Configurator_Test
	{
		private Configurator mConfigurator = null;

		private string mLowestPriorityConfigFile;

		[SetUp]
		public void SetUp()
		{
			string text = Application.dataPath + "/Core/GameSystems/Configurator/UnitTest/";
			mLowestPriorityConfigFile = text + "ApplicationConfig.txt";
			FileHelper.DeleteIfExists(mLowestPriorityConfigFile);
			mConfigurator = new Configurator();
			mConfigurator.SetConfigurationPath(text);
			mConfigurator.Init(true);
		}

		[TearDown]
		public void TearDown()
		{
			FileHelper.DeleteIfExists(Application.dataPath + mLowestPriorityConfigFile);
			mConfigurator = null;
		}

		[Test]
		public void TestCreation()
		{
			Assert.NotNull(mConfigurator);
		}

		[Test]
		public void TestConfigurableSystemAdded()
		{
			Type type = Type.GetType("DisneyMobile.CoreUnitySystems.Test.ConfigurableSystem_Test+ConfigurableSystem");
			IDictionary<string, object> dictionaryForSystem = mConfigurator.GetDictionaryForSystem(type);
			Assert.That((int)dictionaryForSystem["values"].AsDic()["mTestValue3"], Is.EqualTo(42));
		}

		[Test]
		public void TestConfigurableSystemEnabled()
		{
			Type type = Type.GetType("DisneyMobile.CoreUnitySystems.Test.ConfigurableSystem_Test+ConfigurableSystem");
			Assert.IsTrue(mConfigurator.IsSystemEnabled(type));
		}
	}
}
