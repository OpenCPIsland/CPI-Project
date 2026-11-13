using Disney.Kelowna.Common.Environment;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections.Generic;

namespace Disney.Kelowna.Common
{
	public static class ConfigHelper
	{
		public static Disney.Kelowna.Common.Environment.Environment Environment
		{
			get
			{
				return Disney.Kelowna.Common.Environment.Environment.PRODUCTION;
			}
		}

		public static IDictionary<string, object> GetEnvironmentConfig()
		{
			return GetEnvironmentConfig(Environment);
		}

		public static IDictionary<string, object> GetEnvironmentConfig(Disney.Kelowna.Common.Environment.Environment environment)
		{
			Configurator configurator = Service.Get<Configurator>();
			IDictionary<string, object> dictionaryForSystem = configurator.GetDictionaryForSystem("CPRemix");
			IDictionary<string, object> dictionary = (IDictionary<string, object>)dictionaryForSystem["environments"];
			if (!dictionary.ContainsKey(environment.ToString()))
			{
				Log.LogFatal(typeof(ConfigHelper), string.Concat("Environment: ", environment, " does not exist in ApplicationConfig.txt."));
				return null;
			}
			return (IDictionary<string, object>)dictionary[environment.ToString()];
		}

		public static IDictionary<string, object> GetBuildPhaseConfig()
		{
			Configurator configurator = Service.Get<Configurator>();
			IDictionary<string, object> dictionaryForSystem = configurator.GetDictionaryForSystem("CPRemix");
			IDictionary<string, object> dictionary = (IDictionary<string, object>)dictionaryForSystem["buildphaseconfig"];
			return (IDictionary<string, object>)dictionary[ClientInfo.Instance.BuildPhase.ToLower()];
		}

		public static T GetEnvironmentProperty<T>(string key)
		{
			T value;
			TryGetEnvironmentProperty(key, out value);
			return value;
		}

		public static bool TryGetEnvironmentProperty<T>(string key, out T value)
		{
			value = default(T);
			IDictionary<string, object> environmentConfig = GetEnvironmentConfig();
			object value2;
			if (!environmentConfig.TryGetValue(key, out value2) || !(value2 is T))
			{
				return false;
			}
			value = (T)value2;
			return true;
		}
	}
}
