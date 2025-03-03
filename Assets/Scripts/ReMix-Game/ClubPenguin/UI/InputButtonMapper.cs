using ClubPenguin.Core;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class InputButtonMapper : MonoBehaviour
	{
		public InputEvents.Actions Action;

		private InputButton inputButton;

		private TrayInputButton trayButton;

		private void Start()
		{
			Button componentInParent = GetComponentInParent<Button>();
			inputButton = componentInParent.gameObject.AddComponent<InputButton>();
			inputButton.Action = Action;
			trayButton = componentInParent.GetComponent<TrayInputButton>();
			if (trayButton != null)
			{
				trayButton.OnStateChanged += inputButton.OnButtonStateChanged;
			}
		}

		private void OnDestroy()
		{
			if (inputButton != null)
			{
				if (trayButton != null)
				{
					trayButton.OnStateChanged -= inputButton.OnButtonStateChanged;
				}
				Object.Destroy(inputButton);
			}
		}
	}
}
