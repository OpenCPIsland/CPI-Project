using ClubPenguin.Adventure;
using ClubPenguin.Progression;
using Disney.MobileNetwork;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class MascotLevelDisplay : MonoBehaviour
	{
		public Text LevelText;

		public MascotDefinitionKey MascotKey;

		public RadialProgressBar ExperienceProgressBar;

		public GameObject MaxLevelText;

		public Image ProgressBarGraphic;

		private void Awake()
		{
			if (MaxLevelText != null)
			{
				MaxLevelText.SetActive(false);
			}
			LevelText.text = "";
		}

		public int SetUpMascotLevel(bool isLocalPlayer, ProfileData profileData = null, long mascotXPLevel = -1L)
		{
			MascotDefinition definition = Service.Get<MascotService>().GetMascot(MascotKey.Id).Definition;
			ProgressionService progressionService = Service.Get<ProgressionService>();
			int result = 0;
			float num = 0f;
			bool flag = false;
			if (isLocalPlayer)
			{
				if (mascotXPLevel > -1)
				{
					result = ProgressionService.GetMascotLevelFromXP(mascotXPLevel);
					num = ProgressionService.GetMascotLevelPercentFromXP(mascotXPLevel);
					flag = progressionService.IsMascotMaxLevel(definition.name, mascotXPLevel);
				}
				else
				{
					result = progressionService.MascotLevel(definition.name);
					num = progressionService.MascotLevelPercent(definition.name);
					flag = progressionService.IsMascotMaxLevel(definition.name);
				}
			}
			else
			{
				if (profileData == null)
				{
					throw new ArgumentNullException("profileData", "Profile data cannot be null if we are not viewing the local player");
				}
				if (MaxLevelText != null)
				{
					MaxLevelText.SetActive(false);
				}
				long value;
				if (profileData.MascotXP != null && profileData.MascotXP.TryGetValue(definition.name, out value))
				{
					result = ProgressionService.GetMascotLevelFromXP(value);
					num = ProgressionService.GetMascotLevelPercentFromXP(value);
					flag = progressionService.IsMascotMaxLevel(definition.name, value);
				}
			}
			if (MaxLevelText != null && ProgressBarGraphic != null)
			{
				if (flag)
				{
					MaxLevelText.SetActive(true);
					ProgressBarGraphic.color = new Color(1f, 1f, 1f, 0.4f);
				}
				else
				{
					MaxLevelText.SetActive(false);
					ProgressBarGraphic.color = new Color(1f, 1f, 1f, 1f);
				}
			}
			LevelText.text = result.ToString();
			if (ExperienceProgressBar != null)
			{
				if (num >= 0.95f && num < 1f)
				{
					ExperienceProgressBar.AnimateProgress(0.95f);
				}
				else
				{
					ExperienceProgressBar.AnimateProgress(num);
				}
			}
			return result;
		}
	}
}
