


using UnityEngine;

namespace ClubPenguin.Core
{
	public abstract class AspectRatioSpecificSettingsComponent<TComponent, TSettings> : RuntimeSettingsComponent<TComponent, TSettings, AspectRatioType> where TComponent : Component where TSettings : AbstractAspectRatioSpecificSettings
	{
		protected override TSettings getRuntimeSettings()
		{
			return PlatformUtils.FindAspectRatioSettings(runtimeSettings);
		}
	}
}
