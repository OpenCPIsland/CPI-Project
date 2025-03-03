using UnityEngine;

namespace ClubPenguin.UI
{
	[RequireComponent(typeof(InputFieldFormSubmitButton))]
	public class InputFieldFormValidatedEventSource : AbstractEventSource
	{
		private InputFieldFormSubmitButton submitButton;

		private void Awake()
		{
			submitButton = GetComponent<InputFieldFormSubmitButton>();
		}

		private void OnEnable()
		{
			submitButton.OnValidated.AddListener(base.sendEvent);
		}

		private void OnDisable()
		{
			submitButton.OnValidated.RemoveListener(base.sendEvent);
		}
	}
}
