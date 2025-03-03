using ClubPenguin.Gui;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class PenguinProfileStateHandler : MonoBehaviour
	{
		private enum PenguinProfileState
		{
			Colour,
			Profile,
			Progress
		}

		private const string PROFILE_TITLE_TOKEN = "Profile";

		private const string COLOUR_TITLE_TOKEN = "Colour";

		private const string PROGRESS_TITLE_TOKEN = "Progress";

		public Button ProfileButton;

		public Button ColourButton;

		public Button ProgressButton;

		private PenguinProfileState currentState;

		public void OnStateChanged(string newStateString)
		{
			switch ((PenguinProfileState)Enum.Parse(typeof(PenguinProfileState), newStateString))
			{
			case PenguinProfileState.Profile:
				ProfileButton.GetComponent<TintToggleGroupButton>().OnClick();
				ProfileButton.GetComponent<TintToggleGroupButton_Text>().OnClick();
				break;
			case PenguinProfileState.Colour:
				ColourButton.GetComponent<TintToggleGroupButton>().OnClick();
				ColourButton.GetComponent<TintToggleGroupButton_Text>().OnClick();
				break;
			case PenguinProfileState.Progress:
				ProgressButton.GetComponent<TintToggleGroupButton>().OnClick();
				ProgressButton.GetComponent<TintToggleGroupButton_Text>().OnClick();
				break;
			}
		}
	}
}
