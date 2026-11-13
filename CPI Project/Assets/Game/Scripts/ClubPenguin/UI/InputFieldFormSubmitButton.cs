using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace ClubPenguin.UI
{
	public class InputFieldFormSubmitButton : MonoBehaviour, IPointerUpHandler, IEventSystemHandler
	{
		private enum State
		{
			NotValidated,
			Validated
		}

		[Serializable]
		public class FormValidatedEvent : UnityEvent
		{
		}

		[SerializeField]
		private FormValidatedEvent onValidated = new FormValidatedEvent();

		[HideInInspector]
		public InputFieldForm InputForm;

		[SerializeField]
		private bool hideSubmitButton = true;

		private State currentState = State.NotValidated;

		private bool autoSubmit;

		public FormValidatedEvent OnValidated
		{
			get
			{
				return onValidated;
			}
		}

		public bool HideSubmitButton
		{
			get
			{
				return hideSubmitButton;
			}
			set
			{
				if (hideSubmitButton != value)
				{
					hideSubmitButton = value;
					base.gameObject.SetActive(!HideSubmitButton || currentState == State.Validated);
				}
			}
		}

		private void Start()
		{
			autoSubmit = false;
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			autoSubmit = true;
		}

		public void SetWaitingForValidation()
		{
			autoSubmit = true;
		}

		public void UpdateLoop()
		{
			updateState();
			updateCheckWaitingForValidation();
		}

		private void updateState()
		{
			if (InputForm != null)
			{
				if (InputForm.CheckForValidationErrors())
				{
					autoSubmit = false;
					currentState = State.NotValidated;
					base.gameObject.SetActive(!HideSubmitButton);
				}
				else if (currentState != State.Validated && InputForm.IsValidationComplete())
				{
					setValidated();
				}
			}
		}

		private void updateCheckWaitingForValidation()
		{
			if (autoSubmit && currentState == State.Validated)
			{
				autoSubmit = false;
				submit();
			}
		}

		private void setValidated()
		{
			currentState = State.Validated;
			base.gameObject.SetActive(true);
		}

		public void SetInputForm(InputFieldForm form)
		{
			currentState = State.NotValidated;
			InputForm = form;
			base.gameObject.SetActive(!HideSubmitButton);
		}

		private void submit()
		{
			OnValidated.Invoke();
		}
	}
}
