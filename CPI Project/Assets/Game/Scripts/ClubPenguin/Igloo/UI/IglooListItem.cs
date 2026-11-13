using ClubPenguin.Analytics;
using ClubPenguin.Core;
using ClubPenguin.Net.Domain;
using ClubPenguin.UI;
using DevonLocalization.Core;
using Disney.Kelowna.Common.DataModel;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.Igloo.UI
{
	public class IglooListItem : AbstractPlayerListItem
	{
		private const int POPULATION_ICON_OFF_INDEX = 0;

		private const int POPULATION_ICON_ON_INDEX = 1;

		public GameObject PopulationBar;

		public GameObject PrivateLabel;

		public GameObject FullLabel;

		public GameObject YouAreHereIcon;

		public CanvasGroup PlayerIconCanvasGroup;

		public TintSelector[] PopulationBarIcons;

		public float DisabledAlpha = 0.25f;

		private bool isInThisIgloo;

		public override void SetPlayer(DataEntityHandle handle)
		{
			base.SetPlayer(handle);
		}

		public void SetPopulation(RoomPopulation population, bool isFull, bool publicIgloo)
		{
			if (isFull)
			{
				PopulationBar.SetActive(false);
				FullLabel.SetActive(true);
				setEnabled(false);
			}
			else
			{
				PopulationBar.SetActive(true);
				FullLabel.SetActive(false);
				if (!isInThisIgloo)
				{
					PlayerIconCanvasGroup.alpha = 1f;
				}
			}
			PrivateLabel.SetActive(!publicIgloo);
			UpdatePopulationBar(population);
		}

		public void UpdatePopulationBar(RoomPopulation population)
		{
			int populationScaled = (int)population.populationScaled;
			for (int i = 0; i < PopulationBarIcons.Length; i++)
			{
				if (i < populationScaled)
				{
					PopulationBarIcons[i].SelectColor(1);
				}
				else
				{
					PopulationBarIcons[i].SelectColor(0);
				}
			}
		}

		private void OnDestroy()
		{
		}

		public override void OnPressed()
		{
			if (profileData != null && profileData.HasPublicIgloo)
			{
				Service.Get<ActionConfirmationService>().ConfirmAction(typeof(PlayerCardIglooButton), null, delegate
				{
					joinIgloo();
				});
			}
		}

		private void joinIgloo()
		{
			SceneStateData.SceneState sceneState = SceneStateData.SceneState.Play;
			Service.Get<ZoneTransitionService>().LoadIgloo(profileData.ZoneId, Service.Get<Localizer>().Language, sceneState);
			Service.Get<ICPSwrveService>().Action("igloo", "igloo_list_visit", "other_player");
		}

		public override void UpdateProfileData(ProfileData profileData)
		{
			PopulationBar.SetActive(profileData.HasPublicIgloo);
			PrivateLabel.SetActive(!profileData.HasPublicIgloo);
			FullLabel.SetActive(false);
			if (Service.Get<ZoneTransitionService>().CurrentInstanceId == profileData.ZoneId.instanceId)
			{
				setEnabled(false);
				YouAreHereIcon.SetActive(true);
				isInThisIgloo = true;
			}
			else
			{
				setEnabled(profileData.HasPublicIgloo);
				YouAreHereIcon.SetActive(false);
			}
		}

		private void setEnabled(bool isEnabled)
		{
			PlayerIconCanvasGroup.alpha = (isEnabled ? 1f : DisabledAlpha);
			GetComponentInChildren<Button>().interactable = isEnabled;
		}
	}
}
