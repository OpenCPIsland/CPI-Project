using Disney.Kelowna.Common;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	[RequireComponent(typeof(DisplayNamePopupContentController))]
	public class EditDisplayNameStateHandler : MonoBehaviour
	{
		public GameObject DisplayNamePanel;

		public GameObject EditDisplayNameButton;

		public GameObject EditDisplayNamePanel;

		public GameObject EditDisplayNameButtonsPanel;

		public InputField DisplayNameInputField;

		private bool validationStartHasRun;

		private DisplayNamePopupContentController displayNamePopupContentController;

		private void Start()
		{
			displayNamePopupContentController = GetComponent<DisplayNamePopupContentController>();
			EditDisplayNameButton.SetActive(false);
		}

		public void OnStateChanged(string state)
		{
			switch (state)
			{
			case "DisplayName":
				displayNamePopupContentController.ToggleInteraction(true);
				toggleEditDisplayName(false);
				break;
			case "EditDisplayName":
				toggleEditDisplayName(true);
				if (validationStartHasRun)
				{
					CoroutineRunner.Start(displayNamePopupContentController.DisplayNameInputField.Start(), this, "test.Start");
				}
				validationStartHasRun = true;
				DisplayNameInputField.text = "";
				DisplayNameInputField.ActivateInputField();
				break;
			}
		}

		private void toggleEditDisplayName(bool isActive)
		{
			DisplayNamePanel.SetActive(!isActive);
			EditDisplayNamePanel.SetActive(isActive);
			EditDisplayNameButtonsPanel.SetActive(isActive);
		}

		private void OnDestroy()
		{
			CoroutineRunner.StopAllForOwner(this);
		}
	}
}
