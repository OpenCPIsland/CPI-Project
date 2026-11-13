using ClubPenguin;
using ClubPenguin.Core;
using Disney.MobileNetwork;
using UnityEngine;

namespace Clubpenguin
{
	public class PlayerPrefEditorUtils : MonoBehaviour
	{
		public string PlayerName = "";

		public string PreferenceName = "";

		public bool DeleteNow = false;

		public string Status = "";

		private bool lastDeleteValue = false;

		private string lastPrefNameValue = "";

		private void Awake()
		{
			lastDeleteValue = DeleteNow;
		}

		private bool DeletePref(string prefName)
		{
			if (DeleteNow && !string.IsNullOrEmpty(prefName))
			{
				string fullPreferenceName = getFullPreferenceName();
				if (PlayerPrefs.HasKey(fullPreferenceName))
				{
					PlayerPrefs.DeleteKey(fullPreferenceName);
					return true;
				}
			}
			return false;
		}

		private string getPlayerName()
		{
			string result = PlayerName;
			if (Application.isPlaying && string.IsNullOrEmpty(PlayerName))
			{
				CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
				result = cPDataEntityCollection.GetComponent<DisplayNameData>(cPDataEntityCollection.LocalPlayerHandle).DisplayName;
			}
			return result;
		}

		private string getFullPreferenceName()
		{
			return getPlayerName() + PreferenceName;
		}

		private void OnValidate()
		{
			if (!string.IsNullOrEmpty(PreferenceName) && lastPrefNameValue != PreferenceName)
			{
				string fullPreferenceName = getFullPreferenceName();
				if (PlayerPrefs.HasKey(fullPreferenceName))
				{
					Status = "< key exists >";
				}
				else
				{
					Status = "< key does not exist >";
				}
			}
			if (DeleteNow == lastDeleteValue)
			{
				return;
			}
			if (DeleteNow)
			{
				if (DeletePref(PreferenceName))
				{
					Status = "< key deleted >";
				}
				DeleteNow = false;
				lastDeleteValue = DeleteNow;
			}
			else
			{
				Status = "";
			}
		}
	}
}
