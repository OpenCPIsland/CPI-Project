using ClubPenguin.Input;
using Disney.LaunchPadFramework;
using HutongGames.PlayMaker;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.Adventure
{
	[ActionCategory("GUI")]
	public class PressButtonAction : FsmStateAction
	{
		public string ButtonName;

		public FsmGameObject ButtonObject;

		public FsmEvent SendEvent;

		private Button button;

		private ButtonClickListener buttonClickListener;

		public override void OnEnter()
		{
			GameObject gameObject = string.IsNullOrEmpty(ButtonName) ? ButtonObject.Value : GameObject.Find(ButtonName);
			if (gameObject != null)
			{
				button = gameObject.GetComponent<Button>();
				buttonClickListener = gameObject.GetComponent<ButtonClickListener>();
			}
			if (buttonClickListener != null)
			{
				buttonClickListener.OnClick.AddListener(onClickListener);
				return;
			}
			if (button != null)
			{
				button.onClick.AddListener(onPressed);
				return;
			}
			Disney.LaunchPadFramework.Log.LogError(this, "Unable to find button of name : " + ButtonName);
			Finish();
		}

		private void onClickListener(ButtonClickListener.ClickType clickType)
		{
			onPressed();
		}

		private void onPressed()
		{
			if (SendEvent == null || SendEvent.Name == "None")
			{
				Finish();
			}
			else
			{
				base.Fsm.Event(SendEvent);
			}
		}

		public override void OnExit()
		{
			if (buttonClickListener != null)
			{
				buttonClickListener.OnClick.RemoveListener(onClickListener);
			}
			else if (button != null)
			{
				button.onClick.RemoveListener(onPressed);
			}
		}
	}
}
