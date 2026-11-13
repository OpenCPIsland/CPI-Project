using ClubPenguin.UI;
using UnityEngine;

namespace ClubPenguin.Game.PartyGames
{
	[RequireComponent(typeof(InputButtonMapper))]
	public class FishBucketActionButton : MonoBehaviour
	{
		private void Start()
		{
			TrayInputButtonDisabler buttonDisabler = getButtonDisabler();
			if (buttonDisabler != null)
			{
				buttonDisabler.DisableElement(false);
			}
		}

		private TrayInputButtonDisabler getButtonDisabler()
		{
			return GetComponentInParent<TrayInputButtonDisabler>();
		}

		private void OnDestroy()
		{
			TrayInputButtonDisabler buttonDisabler = getButtonDisabler();
			if (buttonDisabler != null)
			{
				buttonDisabler.EnableElement();
			}
		}
	}
}
