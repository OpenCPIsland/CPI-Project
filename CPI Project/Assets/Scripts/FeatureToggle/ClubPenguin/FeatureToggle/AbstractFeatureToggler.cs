using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.FeatureToggle
{
	internal abstract class AbstractFeatureToggler : MonoBehaviour
	{
		[Tooltip("If this is a non RC build the feature will be enabled ")]
		public FeatureDefinitionKey Feature;

		private EventDispatcher dispatcher;

		public void Awake()
		{
			if (Service.Get<FeatureToggleService>().IsEnabled(Feature))
			{
				onFeatureEnabled();
			}
			else
			{
				onFeatureDisabled();
			}
			dispatcher = Service.Get<EventDispatcher>();
			addListeners();
		}

		public void OnDestroy()
		{
			removeListeners();
		}

		private void addListeners()
		{
			dispatcher.AddListener<FeatureToggleEvents.EnableFeature>(onFeatureEnabled);
			dispatcher.AddListener<FeatureToggleEvents.DisableFeature>(onFeatureDisabled);
		}

		private void removeListeners()
		{
			dispatcher.RemoveListener<FeatureToggleEvents.EnableFeature>(onFeatureEnabled);
			dispatcher.RemoveListener<FeatureToggleEvents.DisableFeature>(onFeatureDisabled);
		}

		private bool onFeatureEnabled(FeatureToggleEvents.EnableFeature evt)
		{
			if (evt.Feature.Id == Feature.Id)
			{
				onFeatureEnabled();
			}
			return false;
		}

		private bool onFeatureDisabled(FeatureToggleEvents.DisableFeature evt)
		{
			if (evt.Feature.Id == Feature.Id)
			{
				onFeatureDisabled();
			}
			return false;
		}

		protected abstract void onFeatureEnabled();

		protected abstract void onFeatureDisabled();
	}
}
