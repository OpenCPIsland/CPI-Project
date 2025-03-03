using DevonLocalization.Core;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	[RequireComponent(typeof(Text))]
	public class AppVersionTextDisplay : MonoBehaviour
	{
		private void Start()
		{
			string arg = EnvironmentManager.BundleVersion.ToString();
			GetComponent<Text>().text = string.Format(Service.Get<Localizer>().GetTokenTranslation("GlobalUI.Settings.Settings.AppVersionText"), arg);
		}
	}
}
