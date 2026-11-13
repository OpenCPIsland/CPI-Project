using Disney.LaunchPadFramework;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Core
{
    public abstract class RuntimeSettingsComponent<TComponent, TSettings, TEnum> : MonoBehaviour where TComponent : Component where TSettings : AbstractRuntimeSettings<TEnum> where TEnum : IConvertible
    {
        [SerializeField]
        internal TSettings[] runtimeSettings;

        private void Awake()
        {
            TComponent component = GetComponent<TComponent>();
            TSettings val = getRuntimeSettings();
            if (val != null)
            {
                applySettings(component, val);
            }
        }

        protected abstract TSettings getRuntimeSettings();

        protected abstract void applySettings(TComponent component, TSettings settings);

        protected virtual void onValidate()
        {
        }

        private void OnValidate()
        {
            if (runtimeSettings != null)
            {
                HashSet<TEnum> hashSet = new HashSet<TEnum>();
                for (int i = 0; i < runtimeSettings.Length; i++)
                {
                    if (!hashSet.Contains(runtimeSettings[i].SettingsType))
                    {
                        hashSet.Add(runtimeSettings[i].SettingsType);
                    }
                    else
                    {
                        Log.LogErrorFormatted(this, "runtimeSettings cannot have multiple entries for the same settings type. {0}", runtimeSettings[i].SettingsType);
                    }
                }
            }
            onValidate();
        }
    }
}
