using ClubPenguin.Core;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class InputHoldButtonMapper : MonoBehaviour
	{
		public InputEvents.ChargeActions Action;

		private InputHoldButton inputHoldButton;

		private void Start()
		{
			Button componentInParent = GetComponentInParent<Button>();
			inputHoldButton = componentInParent.gameObject.AddComponent<InputHoldButton>();
			inputHoldButton.Action = Action;
		}

		private void OnDestroy()
		{
			if (inputHoldButton != null)
			{
				Object.Destroy(inputHoldButton);
			}
		}
	}
}
