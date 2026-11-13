using ClubPenguin.Net;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class ModerationPromptController : MonoBehaviour
	{
		public Text TitleText;

		public Text MessageText;

		public IModerationAlert Alert;

		public event Action<ModerationPromptController> OnDestroyed;

		public void SetAlert(IModerationAlert alert)
		{
			Alert = alert;
			MessageText.text = alert.Text;
		}

		public void OnCloseButtonClicked()
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}

		private void OnDestroy()
		{
			if (this.OnDestroyed != null)
			{
				this.OnDestroyed(this);
			}
		}
	}
}
