using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.TutorialUI
{
	[RequireComponent(typeof(Button))]
	public class TutorialTooltipButton : MonoBehaviour
	{
		public GameObject TooltipPrefab;

		public Vector2 Offset;

		public bool FullscreenClose = true;

		private void Awake()
		{
			GetComponent<Button>().onClick.AddListener(delegate
			{
				onButtonClick();
			});
		}

		private void OnDestroy()
		{
			GetComponent<Button>().onClick.RemoveListener(delegate
			{
				onButtonClick();
			});
		}

		protected virtual void onButtonClick()
		{
			if (TooltipPrefab != null)
			{
				GameObject tooltip = Object.Instantiate(TooltipPrefab);
				Service.Get<EventDispatcher>().DispatchEvent(new TutorialUIEvents.ShowTooltip(tooltip, GetComponent<RectTransform>(), Offset, FullscreenClose));
			}
		}
	}
}
