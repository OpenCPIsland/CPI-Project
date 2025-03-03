using ClubPenguin.Core;
using UnityEngine;

namespace ClubPenguin.UI
{
	public abstract class LayoutSwitcherSettingsComponent<TComponent, TSettings, TLayoutSettings> : MonoBehaviour, ILayoutSettingsSwitcher where TComponent : Component where TSettings : AbstractAspectRatioSpecificSettings where TLayoutSettings : LayoutSwitcherSettings<TSettings>
	{
		public TLayoutSettings Settings;

		private TComponent component;

		private void Awake()
		{
			component = GetComponent<TComponent>();
			ApplySettingsForLayout(Settings.DefaultType);
		}

		public void ApplySettingsForLayout(string layoutType)
		{
			if (Settings != null)
			{
				TSettings settings = Settings.GetSettings(layoutType);
				if (settings != null)
				{
					applySettings(component, settings);
				}
			}
		}

		protected abstract void applySettings(TComponent component, TSettings settings);
	}
}
