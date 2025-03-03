using ClubPenguin.Core;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.SEDFSM;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class KeyboardEventSource : MonoBehaviour
	{
		private enum EventState
		{
			None,
			Pending,
			Invalid
		}

		public string Target;

		public string HiddenEvent;

		public string ShownEvent;

		private EventState currentEventState;

		private void OnEnable()
		{
			Service.Get<EventDispatcher>().AddListener<KeyboardEvents.KeyboardHidden>(onKeyboardHidden);
			Service.Get<EventDispatcher>().AddListener<KeyboardEvents.KeyboardShown>(onKeyboardShown);
		}

		private void OnDisable()
		{
			CoroutineRunner.StopAllForOwner(this);
			Service.Get<EventDispatcher>().RemoveListener<KeyboardEvents.KeyboardHidden>(onKeyboardHidden);
			Service.Get<EventDispatcher>().RemoveListener<KeyboardEvents.KeyboardShown>(onKeyboardShown);
		}

		private bool onKeyboardHidden(KeyboardEvents.KeyboardHidden evt)
		{
			if (currentEventState == EventState.Pending)
			{
				currentEventState = EventState.Invalid;
			}
			else if (currentEventState == EventState.None)
			{
				CoroutineRunner.Start(handleKeyboardHidden(), this, "");
			}
			return false;
		}

		private IEnumerator handleKeyboardHidden()
		{
			currentEventState = EventState.Pending;
			yield return null;
			if (currentEventState == EventState.Pending)
			{
				dispatchEvent(HiddenEvent);
			}
			else if (currentEventState == EventState.Invalid)
			{
				showKeyboard();
			}
			currentEventState = EventState.None;
		}

		private bool onKeyboardShown(KeyboardEvents.KeyboardShown evt)
		{
			if (evt.Height > 0 && currentEventState == EventState.Pending)
			{
				currentEventState = EventState.Invalid;
			}
			else if (evt.Height > 0 && currentEventState == EventState.None)
			{
				CoroutineRunner.Start(handleKeyboardShown(), this, "");
			}
			return false;
		}

		private IEnumerator handleKeyboardShown()
		{
			currentEventState = EventState.Pending;
			yield return null;
			if (currentEventState == EventState.Pending)
			{
				dispatchEvent(ShownEvent);
			}
			else if (currentEventState == EventState.Invalid)
			{
				showKeyboard();
			}
			currentEventState = EventState.None;
		}

		private void showKeyboard()
		{
			InputBarField inputBarField = Object.FindObjectOfType<InputBarField>();
			inputBarField.ShowKeyboard();
		}

		private void dispatchEvent(string evt)
		{
			if (!string.IsNullOrEmpty(evt))
			{
				StateMachineContext componentInParent = GetComponentInParent<StateMachineContext>();
				if (componentInParent != null)
				{
					componentInParent.SendEvent(new ExternalEvent(Target, evt));
				}
				else
				{
					Log.LogError(this, "Could not find a StateMachineContext in the parent");
				}
			}
		}
	}
}
