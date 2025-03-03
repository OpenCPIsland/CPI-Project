using ClubPenguin.UI;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.ContentGates
{
	internal class GatePrefabController : MonoBehaviour
	{
		public InputField AnswerInputField;

		public Text InstructionsText;

		public Text QuestionText;

		public Image ErrorIcon;

		public Button CloseButton;

		public Button SubmitButton;

		private void Start()
		{
			if (CloseButton != null)
			{
				CloseButton.gameObject.AddComponent<BackButton>();
			}
			AnswerInputField.ActivateInputField();
		}
	}
}
