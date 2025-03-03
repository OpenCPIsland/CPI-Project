using ClubPenguin.Adventure;
using ClubPenguin.Progression;
using ClubPenguin.TutorialUI;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class LevelTooltipButton : TutorialTooltipButton
	{
		public GameObject MaxLevelTooltipPrefab;

		public MascotDefinition Mascot;

		protected override void onButtonClick()
		{
			GameObject gameObject = TooltipPrefab;
			ProgressionService progressionService = Service.Get<ProgressionService>();
			if ((Mascot != null && progressionService.IsMascotMaxLevel(Mascot.name)) || (Mascot == null && progressionService.Level == progressionService.MaxUnlockLevel))
			{
				gameObject = MaxLevelTooltipPrefab;
			}
			if (gameObject != null)
			{
				GameObject tooltip = Object.Instantiate(gameObject);
				Service.Get<EventDispatcher>().DispatchEvent(new TutorialUIEvents.ShowTooltip(tooltip, GetComponent<RectTransform>(), Offset, FullscreenClose));
			}
		}
	}
}
