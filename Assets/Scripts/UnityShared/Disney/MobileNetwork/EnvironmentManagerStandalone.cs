using Disney.Kelowna.Common;
using System;
using UnityEngine;

namespace Disney.MobileNetwork
{
	public class EnvironmentManagerStandalone : EnvironmentManager
	{
#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_STANDALONE_LINUX
        protected override string _SKU
		{
			get
			{
				if (string.IsNullOrEmpty(EnvironmentManager.mSKU))
				{
					EnvironmentManager.mSKU = "standalone";
				}
				return EnvironmentManager.mSKU;
			}
		}

		protected override string _SKUPluginManifestJson
		{
			get
			{
				return null;
			}
		}

		protected override void _Init()
		{
			EnvironmentManager.mBundleIdentifier = Application.identifier;
			string version = Application.version;
			version = EnvironmentManager.NormalizeVersionString(version);
			EnvironmentManager.mBundleVersionCode = ClientInfo.Instance.Changelist;
			if (!string.IsNullOrEmpty(version))
			{
				EnvironmentManager.mBundleVersion = new Version(version);
			}
			else
			{
				EnvironmentManager.mBundleVersion = new Version(0, 0, 0);
			}
			BuildSettings.LoadSettings();
		}

		protected override string _GetLocale()
		{
			return "us_en";
		}

		protected override string _GetDeviceLanguage()
		{
			return "en";
		}

		protected override int _GetDiskSpaceFreeMegabytes()
		{
			return 1048576000;
		}

		protected override bool _GetIsMusicPlaying()
		{
			return false;
		}

		protected override bool _GetAreHeadphonesConnected()
		{
			return false;
		}

		protected override bool _GetIsExternalLinksRestricted()
		{
			return false;
		}

		protected override string _GetBuildSettingsJson()
		{
			return null;
		}

		public override void ShowAlert(ShowAlertDelegate showAlertDelegate, string title, string message, string viewButtonText, string cancelButtonText)
		{
			base.ShowAlert(showAlertDelegate, title, message, viewButtonText, cancelButtonText);
		}

		protected override void _ShowStatusBar(bool show, bool useLightColor)
		{
		}
#endif
    }
}
