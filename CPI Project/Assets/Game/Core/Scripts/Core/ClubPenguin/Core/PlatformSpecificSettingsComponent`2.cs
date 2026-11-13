using UnityEngine;

namespace ClubPenguin.Core
{
    public abstract class PlatformSpecificSettingsComponent<TComponent, TSettings> : RuntimeSettingsComponent<TComponent, TSettings, PlatformType> where TComponent : Component where TSettings : AbstractPlatformSpecificSettings
    {
        protected override TSettings getRuntimeSettings()
        {
            return PlatformUtils.FindPlatformSettings(runtimeSettings);
        }
    }
}

