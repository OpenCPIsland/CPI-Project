using Disney.Kelowna.Common;
using Disney.Kelowna.Common.SEDFSM;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using Mix.Native;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class InputBarFieldEditor : InputBarField
	{
		public InputField Input;

		private KeyboardController keyboardController;

		protected override void Awake()
		{
			keyboardController = base.gameObject.AddComponent<KeyboardController>();
			Input.caretWidth = 3;
			base.Awake();
		}

		private void Start()
		{
			Service.Get<EventDispatcher>().DispatchEvent(default(ChatEvents.InputBarLoaded));
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			Input.onValueChanged.AddListener(base.updateText);
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			Input.onValueChanged.RemoveListener(base.updateText);
		}

		public override void SetInputFieldSelected()
		{
			CoroutineRunner.Start(selectInputField(), this, "selectInputField");
			base.SetInputFieldSelected();
			StateMachineContext componentInParent = GetComponentInParent<StateMachineContext>();
			if (componentInParent != null && componentInParent.ContainsStateMachine("Root"))
			{
				componentInParent.SendEvent(new ExternalEvent("Root", "chat_input"));
			}
		}

		public override void SetInputFieldDeselected()
		{
			Input.interactable = false;
			Input.DeactivateInputField();
		}

		public override void SetCharacterLimit(int limit)
		{
			Input.characterLimit = limit;
		}

		public override void setViewText(string value)
		{
			Input.text = value;
		}

		public override void SetPlaceholderText(string text)
		{
			Input.placeholder.GetComponent<Text>().text = text;
		}

		public override void ShowKeyboard()
		{
			keyboardController.ShowKeyboard(0, NativeKeyboardReturnKey.Default, base.ShowSuggestions);
		}

		public override void HideKeyboard()
		{
			keyboardController.HideKeyboard();
		}

		public override void Clear()
		{
			Input.text = "";
			Input.Select();
			Input.ActivateInputField();
		}

		public override void Reset()
		{
			Clear();
		}

		protected override Font GetFont()
		{
			return Input.textComponent.font;
		}

		protected override void onEmoteSelected(ChatEvents.EmoteSelected evt)
		{
			Input.text += evt.EmoteString;
			CoroutineRunner.Start(selectInputField(), this, "selectInputField");
			base.SetInputFieldSelected();
		}

		private IEnumerator selectInputField()
		{
			Input.interactable = true;
			Input.ActivateInputField();
			Input.Select();
			Input.textComponent.enabled = false;
			yield return new WaitForEndOfFrame();
			Input.textComponent.enabled = true;
			Input.MoveTextEnd(false);
		}
	}
}
