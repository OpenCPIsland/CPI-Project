using Disney.Kelowna.Common;
using System.Collections;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class InputFieldForm : MonoBehaviour
	{
		[Header("Fields")]
		[SerializeField]
		private InputFieldValidator[] validators = new InputFieldValidator[0];

		[SerializeField]
		private InputFieldValidator autoSelectedValidator = null;

		[Header("Submission")]
		[SerializeField]
		private InputFieldFormSubmitButton submitButton = null;

		private InputFieldValidator lastValidator;

		private bool validatorsProcessedPreviously;

		private void setupSubmitButtonInputForm()
		{
			if (submitButton != null)
			{
				submitButton.SetInputForm(this);
			}
		}

		private void OnEnable()
		{
			setupSubmitButtonInputForm();
			int num = validators.Length;
			for (int i = 0; i < num; i++)
			{
				lastValidator = validators[i];
				lastValidator.TextInput.OnKeyboardDone += onInputFieldKeyboardDone;
				if (validatorsProcessedPreviously)
				{
					CoroutineRunner.Start(lastValidator.Start(), this, "InputFieldForm.InputFieldValidator.Start");
				}
			}
			if (autoSelectedValidator != null && !autoSelectedValidator.HasError && !autoSelectedValidator.IsValidationComplete)
			{
				CoroutineRunner.Start(autoSelectValidator(), this, "InputFieldForm.autoSelectValidator");
			}
			validatorsProcessedPreviously = true;
		}

		private void OnDisable()
		{
			int num = validators.Length;
			for (int i = 0; i < num; i++)
			{
				validators[i].TextInput.OnKeyboardDone -= onInputFieldKeyboardDone;
			}
			CoroutineRunner.StopAllForOwner(this);
		}

		private void Update()
		{
			if (submitButton != null)
			{
				submitButton.UpdateLoop();
			}
		}

		private IEnumerator autoSelectValidator()
		{
			yield return null;
			autoSelectedValidator.TextInput.Select();
		}

		private void onInputFieldKeyboardDone(InputFieldEx inputField)
		{
			if (inputField == lastValidator.TextInput)
			{
				startValidation();
			}
			else
			{
				selectNextValidator(inputField);
			}
		}

		private void startValidation()
		{
			for (int i = 0; i < validators.Length; i++)
			{
				InputFieldValidator inputFieldValidator = validators[i];
				if (!inputFieldValidator.IsValidationInProgress && !inputFieldValidator.IsValidationComplete)
				{
					inputFieldValidator.StartValidation();
				}
			}
			if (submitButton != null)
			{
				submitButton.SetWaitingForValidation();
			}
		}

		private void selectNextValidator(InputFieldEx previousInputField)
		{
			int num = validators.Length - 1;
			InputFieldValidator inputFieldValidator = null;
			int num2 = num;
			while (num2 >= 0 && !(validators[num2].TextInput == previousInputField))
			{
				inputFieldValidator = validators[num2];
				num2--;
			}
			if (inputFieldValidator != null)
			{
				inputFieldValidator.TextInput.Select();
			}
		}

		public bool IsValidationComplete()
		{
			bool flag = true;
			for (int i = 0; i < validators.Length; i++)
			{
				flag &= validators[i].IsValidationComplete;
			}
			return flag;
		}

		public bool CheckForValidationErrors()
		{
			bool flag = false;
			for (int i = 0; i < validators.Length; i++)
			{
				flag |= validators[i].HasError;
			}
			return flag;
		}
	}
}
