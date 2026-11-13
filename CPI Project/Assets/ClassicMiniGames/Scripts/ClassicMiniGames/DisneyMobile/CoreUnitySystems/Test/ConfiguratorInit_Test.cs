using DisneyMobile.CoreUnitySystems.Utility;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace DisneyMobile.CoreUnitySystems.Test
{
	[TestFixture]
	public class ConfiguratorInit_Test
	{
		[Test]
		public void TestCreation()
		{
			string text = Application.dataPath + "/Core/GameSystems/Configurator/UnitTest/";
			string text2 = text + "ApplicationConfig.txt";
			FileHelper.DeleteIfExists(text2);
			Configurator configurator = new Configurator();
			Assert.IsNotNull(configurator);
			configurator.SetConfigurationPath(text);
			configurator.Init(true);
			Assert.IsTrue(File.Exists(text2));
			configurator = null;
			configurator = new Configurator();
			Assert.IsNotNull(configurator);
			configurator.SetConfigurationPath(text);
			configurator.Init(true);
			Type type = Type.GetType("DisneyMobile.CoreUnitySystems.Test.ConfigurableSystem_Test+ConfigurableSystem");
			IDictionary<string, object> dictionaryForSystem = configurator.GetDictionaryForSystem(type);
			Assert.That((int)dictionaryForSystem["values"].AsDic()["mTestValue3"], Is.EqualTo(42));
			FileHelper.DeleteIfExists(text2);
			configurator = null;
		}
	}
}
