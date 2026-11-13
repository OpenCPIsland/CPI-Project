using DevonLocalization.Core;
using Disney.MobileNetwork;
using TMPro;
using UnityEngine;

namespace ClubPenguin.UI
{
	[RequireComponent(typeof(TextMeshProUGUI))]
	public class AppVersionTextDisplay : MonoBehaviour
	{
		private void Start()
		{
			string version = Application.version;
			string translation = Service.Get<Localizer>().GetTokenTranslation("GlobalUI.Settings.Settings.AppVersionText");
			if (!string.IsNullOrEmpty(translation) && translation != "GlobalUI.Settings.Settings.AppVersionText")
			{
				GetComponent<TextMeshProUGUI>().text = string.Format(translation, version);
			}
			else
			{
				GetComponent<TextMeshProUGUI>().text = "v1.17.1";
			}
		}
	}
}
