using System;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class InputFieldComparisonValidator : InputFieldValidator
	{
		public InputFieldValidator[] InputFieldsToCompare;

		[Tooltip("Do an exact match comparison of a string")]
		public bool ExactMatch = true;

		[Tooltip("Reversed logic: true = found string triggers error")]
		public bool ReverseCompare = false;

		[Tooltip("Clear the value in this field if the comparison fields fail their validation")]
		public bool ClearOnOtherFieldsError = false;

		public string i18nErrorMessage;

		private InputFieldComparisonValidationAction validationAction = null;

		protected override void onAwake()
		{
			base.onAwake();
			bool flag = false;
			InputFieldValidatonAction[] validationActions = ValidationActions;
			foreach (InputFieldValidatonAction inputFieldValidatonAction in validationActions)
			{
				if (inputFieldValidatonAction is InputFieldComparisonValidationAction)
				{
					flag = true;
					validationAction = (inputFieldValidatonAction as InputFieldComparisonValidationAction);
					break;
				}
			}
			if (!flag)
			{
				validationAction = ScriptableObject.CreateInstance<InputFieldComparisonValidationAction>();
				validationAction.ExactMatch = ExactMatch;
				validationAction.ReverseCompare = ReverseCompare;
				Array.Resize(ref ValidationActions, ValidationActions.Length + 1);
				ValidationActions[ValidationActions.Length - 1] = validationAction;
			}
			updateStringsToCompare();
			validationAction.i18nErrorMessage = i18nErrorMessage;
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			if (ClearOnOtherFieldsError)
			{
				for (int i = 0; i < InputFieldsToCompare.Length; i++)
				{
					InputFieldsToCompare[i].OnValidationError.AddListener(onOtherFieldValidationError);
					InputFieldsToCompare[i].OnValidationSuccess.AddListener(onOtherFieldValidationSuccess);
				}
			}
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			if (ClearOnOtherFieldsError)
			{
				for (int i = 0; i < InputFieldsToCompare.Length; i++)
				{
					InputFieldsToCompare[i].OnValidationError.RemoveListener(onOtherFieldValidationError);
					InputFieldsToCompare[i].OnValidationSuccess.RemoveListener(onOtherFieldValidationSuccess);
				}
			}
		}

		public override void StartValidation()
		{
			updateStringsToCompare();
			base.StartValidation();
		}

		private void updateStringsToCompare()
		{
			string[] array = new string[InputFieldsToCompare.Length];
			for (int i = 0; i < InputFieldsToCompare.Length; i++)
			{
				array[i] = InputFieldsToCompare[i].TextInput.text;
			}
			validationAction.StringsToCompare = array;
		}

		private void onOtherFieldValidationError(string errorMessage)
		{
			HasError = false;
			TextInput.text = "";
			TextInput.enabled = false;
			IsValidationComplete = false;
			ValidationInputController component = GetComponent<ValidationInputController>();
			ValidationIconController component2 = GetComponent<ValidationIconController>();
			if (component != null)
			{
				component.Reset();
			}
			if (component2 != null)
			{
				component2.Reset();
			}
		}

		private void onOtherFieldValidationSuccess()
		{
			TextInput.enabled = true;
			updateStringsToCompare();
		}
	}
}
