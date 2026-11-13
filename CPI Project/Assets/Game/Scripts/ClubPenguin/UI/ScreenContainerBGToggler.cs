using UnityEngine;

namespace ClubPenguin.UI
{
	public class ScreenContainerBGToggler : MonoBehaviour
	{
		private GameObject bg;

		private ScreenContainerStateHandler screenContainerStateHandler;

		private void Start()
		{
			screenContainerStateHandler = GetComponentInParent<ScreenContainerStateHandler>();
			bg = screenContainerStateHandler.GetComponentInChildren<ScreenContainerBG>(true).gameObject;
			bg.gameObject.SetActive(false);
		}

		private void OnDisable()
		{
			if (!bg.gameObject.IsDestroyed() && screenContainerStateHandler.ShowScreenContainerBG)
			{
				bg.gameObject.SetActive(true);
			}
		}
	}
}
