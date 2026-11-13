using System;
using System.Collections;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class CountDownTrayNotification : TrayNotification
	{
		private string rawMessage;

		public override void Show(DNotification data)
		{
			rawMessage = data.Message;
			base.Show(data);
		}

		protected override IEnumerator notificationDisplayTime(float delaySeconds)
		{
			while (delaySeconds > 0f)
			{
				updateText((int)delaySeconds);
				yield return new WaitForSeconds(Math.Min(1f, delaySeconds));
				delaySeconds -= 1f;
			}
			yield return animateAway();
		}

		private void updateText(int secondsLeft)
		{
			messageWithoutButtons.text = string.Format(rawMessage, secondsLeft);
			messageWithButtons.text = string.Format(rawMessage, secondsLeft);
		}
	}
}
