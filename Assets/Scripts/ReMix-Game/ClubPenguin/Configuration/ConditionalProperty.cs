using ClubPenguin.Analytics;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.Configuration
{
	public abstract class ConditionalProperty
	{
		public const string LOGGED_PROPERTIES_KEY_FORMAT = "cp.ConditionalPropertyLogged.{0}";

		public string AnalyticsTierName = "default";

		public string Key
		{
			get;
			protected set;
		}

		public abstract string ValueToString();
	}
	public class ConditionalProperty<T> : ConditionalProperty
	{
		private readonly T defaultValue;

		public ConditionalTier<T> Tier;

		public T Value
		{
			get
			{
				return (Tier != null) ? Tier.DynamicValue : defaultValue;
			}
			internal set
			{
				Tier.DynamicValue = value;
			}
		}

		public ConditionalProperty(ConditionalDefinition<T> definition)
		{
			base.Key = definition.name;
			defaultValue = definition.DefaultValue;
			CalculateTier(definition);
		}

		private void CalculateTier(ConditionalDefinition<T> definition)
		{
			ConditionalTier<T>[] tiers = definition.Tiers;
			foreach (ConditionalTier<T> conditionalTier in tiers)
			{
				if (ConditionsMeetForTier(conditionalTier))
				{
					Tier = conditionalTier;
					AnalyticsTierName = (string.IsNullOrEmpty(conditionalTier.AnalyticsName) ? conditionalTier.name : conditionalTier.AnalyticsName);
					break;
				}
			}
			string key = string.Format("cp.ConditionalPropertyLogged.{0}", base.Key);
			if (PlayerPrefs.GetInt(key, 0) == 0 && definition.SendAnalytics)
			{
				PlayerPrefs.SetInt(key, 1);
				if (Service.IsSet<ICPSwrveService>())
				{
					string str = base.Key.ToLower();
					string tier = AnalyticsTierName.ToLower();
					Service.Get<ICPSwrveService>().Action("game." + str, tier);
				}
			}
		}

		private bool ConditionsMeetForTier(ConditionalTier<T> tier)
		{
			return tier.Condition.IsSatisfied();
		}

		public override string ValueToString()
		{
			return (Tier != null) ? Tier.ToString() : ("? " + defaultValue);
		}

		public override string ToString()
		{
			return string.Format("[Key: {0}; TierName: {1}; Value: {2}]", base.Key, AnalyticsTierName, Value);
		}
	}
}
