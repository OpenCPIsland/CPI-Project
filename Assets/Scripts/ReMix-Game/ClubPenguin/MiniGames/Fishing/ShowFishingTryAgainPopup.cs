using ClubPenguin.UI;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections;
using UnityEngine;

namespace ClubPenguin.MiniGames.Fishing
{
	public class ShowFishingTryAgainPopup
	{
		private const string TOO_EARLY_TOKEN = "Minigame.Fishing.TryAgain.TooEarly";

		private const string TOO_LATE_TOKEN = "Minigame.Fishing.TryAgain.TooLate";

		private readonly float popupTime;

		private readonly bool tooEarly;

		private static PrefabContentKey tryAgainContentKey = new PrefabContentKey("Fishing/TryAgainPrefab");

		public event System.Action PopupDismissed;

		public ShowFishingTryAgainPopup(float popupTime, bool tooEarly)
		{
			this.popupTime = popupTime;
			this.tooEarly = tooEarly;
		}

		public void Init()
		{
			CoroutineRunner.Start(loadTryAgainPopup(), this, "loadTryAgainPopup");
		}

		private IEnumerator loadTryAgainPopup()
		{
			AssetRequest<GameObject> assetRequestTryAgainPopup = Content.LoadAsync(tryAgainContentKey);
			yield return assetRequestTryAgainPopup;
			string message = (!tooEarly) ? Service.Get<Localizer>().GetTokenTranslation("Minigame.Fishing.TryAgain.TooLate") : Service.Get<Localizer>().GetTokenTranslation("Minigame.Fishing.TryAgain.TooEarly");
			if (assetRequestTryAgainPopup != null)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(assetRequestTryAgainPopup.Asset);
				FishingTryAgainPopup component = gameObject.GetComponent<FishingTryAgainPopup>();
				if (component != null)
				{
					component.PopupDismissed += onPopupDismissed;
					component.Init(popupTime, message);
					Service.Get<EventDispatcher>().DispatchEvent(new PopupEvents.ShowCameraSpacePopup(gameObject));
				}
				else
				{
					Log.LogError(this, "Unable to load the TryAgain Popup asset.");
					onPopupDismissed();
				}
			}
		}

		private void onPopupDismissed()
		{
			if (this.PopupDismissed != null)
			{
				this.PopupDismissed();
				this.PopupDismissed = null;
			}
		}
	}
}
