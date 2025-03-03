using UnityEngine;

namespace ClubPenguin.UI
{
	public class InputFieldValidatorGroup : MonoBehaviour
	{
		public InputFieldValidator[] Validators;

		public void StartValidation()
		{
			for (int i = 0; i < Validators.Length; i++)
			{
				InputFieldValidator inputFieldValidator = Validators[i];
				if (!inputFieldValidator.IsValidationInProgress && !inputFieldValidator.IsValidationComplete)
				{
					inputFieldValidator.StartValidation();
				}
			}
		}

		public bool IsValidationComplete()
		{
			bool flag = true;
			for (int i = 0; i < Validators.Length; i++)
			{
				flag &= Validators[i].IsValidationComplete;
			}
			return flag;
		}

		public bool CheckForValidationErrors()
		{
			bool flag = false;
			for (int i = 0; i < Validators.Length; i++)
			{
				flag |= Validators[i].HasError;
			}
			return flag;
		}

		public void ResetValidationComplete()
		{
			for (int i = 0; i < Validators.Length; i++)
			{
				Validators[i].IsValidationComplete = false;
			}
		}
	}
}
