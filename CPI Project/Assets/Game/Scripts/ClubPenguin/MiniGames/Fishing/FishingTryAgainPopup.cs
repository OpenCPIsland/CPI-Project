using Disney.Kelowna.Common;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.MiniGames.Fishing
{
	public class FishingTryAgainPopup : MonoBehaviour
	{
		[SerializeField]
		private Text isTooEarlyText;

		[SerializeField]
		private float hideTextTime;

		public event Action PopupDismissed;

		public void Init(float delayTime, string message)
		{
			isTooEarlyText.text = message;
			CoroutineRunner.Start(hideText(), this, "hideText");
			CoroutineRunner.Start(dismissPopup(delayTime), this, "dismissPopup");
		}

		private IEnumerator hideText()
		{
			yield return new WaitForSeconds(hideTextTime);
			isTooEarlyText.enabled = false;
		}

		private IEnumerator dismissPopup(float delayTime)
		{
			yield return new WaitForSeconds(delayTime);
			if (this.PopupDismissed != null)
			{
				this.PopupDismissed();
				this.PopupDismissed = null;
			}
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
