using ClubPenguin.Analytics;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using Disney.Native;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace ClubPenguin.UI
{
	public class InputFieldValidator : ScriptableActionPlayer, IFormValidationItem
	{
		[Header("Validation Actions will be executed in the order of the list, starting at 0")]
		public InputFieldValidatonAction[] ValidationActions;

		public InputFieldEx TextInput;

		[Tooltip("Used for sending BI events should be Create, Login, DisplayName etc")]
		public string GameActionContext;

		[HideInInspector]
		public bool IsValidationComplete = false;

		[HideInInspector]
		public bool IsValidationInProgress = false;

		[HideInInspector]
		public bool HasError = false;

		public ValidationEvent OnValidationError;

		public UnityEvent OnValidationSuccess;

		private ScriptableActionPause pauseAction;

		private int actionIndex = 0;

		private string lastKeyboardDoneText;

		protected override void onAwake()
		{
			if (!Service.IsSet<Localizer>())
			{
				Service.Set(new Localizer());
			}
			if (!(Action is ScriptableActionPause))
			{
				throw new Exception("First Action in a validator must be a ScriptableActionPause action");
			}
			pauseAction = (Action as ScriptableActionPause);
		}

		protected virtual void OnEnable()
		{
			Reset();
			TextInput.onDeselected += onDeselected;
			TextInput.OnKeyboardDone += onKeyboardDone;
		}

		protected virtual void OnDisable()
		{
			TextInput.onDeselected -= onDeselected;
			TextInput.OnKeyboardDone -= onKeyboardDone;
		}

		protected void onDeselected(string text)
		{
			if (lastKeyboardDoneText == null || lastKeyboardDoneText != text)
			{
				StartValidation();
				lastKeyboardDoneText = null;
			}
		}

		private void onKeyboardDone(InputFieldEx inputField)
		{
			lastKeyboardDoneText = inputField.text;
			StartValidation();
		}

		public virtual void StartValidation()
		{
			if (!IsValidationInProgress && !(Action is ScriptableActionPause))
			{
				Reset();
			}
			IsValidationInProgress = true;
			IsValidationComplete = false;
			HasError = false;
			base.ActionIsFinished = true;
		}

		protected override void onActionDone()
		{
			if (Action is ScriptableActionPause)
			{
				setNextAction();
				return;
			}
			InputFieldValidatonAction inputFieldValidatonAction = Action as InputFieldValidatonAction;
			HasError = inputFieldValidatonAction.HasError;
			if (HasError)
			{
				sendBIData(false, inputFieldValidatonAction.i18nErrorMessage);
				ShowError(inputFieldValidatonAction.GetErrorMessage());
				Reset();
				IsValidationComplete = true;
				IsValidationInProgress = false;
			}
			else
			{
				setNextAction();
			}
		}

		private void setNextAction()
		{
			if (actionIndex < ValidationActions.Length)
			{
				base.NextAction = ValidationActions[actionIndex];
				actionIndex++;
				return;
			}
			Reset();
			IsValidationComplete = true;
			IsValidationInProgress = false;
			sendBIData(true, "");
			OnValidationSuccess.Invoke();
		}

		public void ShowError(string errorMessage)
		{
			if (MonoSingleton<NativeAccessibilityManager>.Instance.Native.GetAccessibilityLevel() > 0)
			{
				HasError = true;
			}
			OnValidationError.Invoke(errorMessage);
		}

		public void Reset()
		{
			actionIndex = 0;
			base.NextAction = pauseAction;
		}

		private void sendBIData(bool status, string message)
		{
			if (status)
			{
				Service.Get<ICPSwrveService>().Action("game." + GameActionContext + "." + base.gameObject.name + "_validation", "success");
			}
			else
			{
				Service.Get<ICPSwrveService>().Action("game." + GameActionContext + "." + base.gameObject.name + "_validation", "failed", message);
			}
		}
	}
}
