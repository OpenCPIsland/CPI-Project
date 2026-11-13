using ClubPenguin.Core;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections.Generic;

namespace ClubPenguin.FeatureToggle
{
	public class FeatureToggleService
	{
		private Dictionary<string, FeatureDefinition> features;

		private Dictionary<string, DevCacheableType<bool>> featureSettings;

		private ICommonGameSettings gameSettings;

		private EventDispatcher dispatcher;

		public FeatureToggleService()
		{
			gameSettings = Service.Get<ICommonGameSettings>();
			dispatcher = Service.Get<EventDispatcher>();
			features = Service.Get<IGameData>().Get<Dictionary<string, FeatureDefinition>>();
			featureSettings = new Dictionary<string, DevCacheableType<bool>>();
			foreach (FeatureDefinition value in features.Values)
			{
				init(value);
			}
		}

		public bool IsEnabled(FeatureDefinitionKey def)
		{
			return false;
		}

		public bool IsEnabled(FeatureDefinition def)
		{
			return false;
		}

		private void init(FeatureDefinition def)
		{
			disable(def);
		}

		private void disable(FeatureDefinition def)
		{
			dispatcher.DispatchEvent(new FeatureToggleEvents.DisableFeature(def));
		}
	}
}
