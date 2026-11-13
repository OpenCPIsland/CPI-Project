using DevonLocalization.Core;
using Disney.MobileNetwork;
using TMPro;
using UnityEngine;

namespace ClubPenguin.UI
{
	[RequireComponent(typeof(TextMeshProUGUI))]
	public class UnityVersionTextDisplay : MonoBehaviour
	{
		private void Start()
		{
			string version = Application.unityVersion;
			string translation = Service.Get<Localizer>().GetTokenTranslation("GlobalUI.Settings.Settings.UnityVersionText");
			if (!string.IsNullOrEmpty(translation) && translation != "GlobalUI.Settings.Settings.UnityVersionText")
			{
				GetComponent<TextMeshProUGUI>().text = string.Format(translation, version);
			}
		}
	}
}
