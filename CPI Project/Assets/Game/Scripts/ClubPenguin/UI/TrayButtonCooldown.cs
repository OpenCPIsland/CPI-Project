using Disney.Kelowna.Common;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class TrayButtonCooldown : MonoBehaviour
	{
		public float CooldownSeconds;

		private Button parentButton;

		private TrayInputButton trayInputButton;

		private TrayInputButton.ButtonState previousState;

		private void Start()
		{
			parentButton = GetComponentInParent<Button>();
			trayInputButton = GetComponentInParent<TrayInputButton>();
			parentButton.onClick.AddListener(onButtonClicked);
		}

		private void onButtonClicked()
		{
			previousState = trayInputButton.CurrentState;
			trayInputButton.SetState(TrayInputButton.ButtonState.Disabled);
			CoroutineRunner.Start(delayedCooldown(), this, "delayedCooldown");
		}

		private IEnumerator delayedCooldown()
		{
			yield return new WaitForSeconds(CooldownSeconds);
			trayInputButton.SetState(previousState);
		}

		private void OnDestroy()
		{
			parentButton.onClick.RemoveListener(onButtonClicked);
			CoroutineRunner.StopAllForOwner(this);
		}
	}
}
