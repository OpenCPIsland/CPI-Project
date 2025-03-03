using ClubPenguin.Progression;
using Disney.MobileNetwork;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class PassportCodeController : MonoBehaviour
	{
		public Text NameText;

		public Text AgeText;

		public Text LevelText;

		public void SetDisplayNameText(string displayName)
		{
			string text = (displayName.Length <= 12) ? displayName : displayName.Substring(0, 12);
			NameText.text = text;
		}

		public void SetAgeText(int age)
		{
			AgeText.text = age.ToString();
		}

		public void SetUpLevelText(bool isLocalPlayer, ProfileData profileData = null)
		{
			int num = 0;
			if (isLocalPlayer)
			{
				num = Service.Get<ProgressionService>().Level;
			}
			else
			{
				if (profileData == null)
				{
					throw new ArgumentNullException("profileData", "Profile data cannot be null if we are not viewing the local player");
				}
				if (profileData.MascotXP != null)
				{
					foreach (KeyValuePair<string, long> item in profileData.MascotXP)
					{
						num += ProgressionService.GetMascotLevelFromXP(item.Value);
					}
				}
			}
			LevelText.text = num.ToString();
		}
	}
}
