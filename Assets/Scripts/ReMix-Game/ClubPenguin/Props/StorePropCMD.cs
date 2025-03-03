using ClubPenguin.UI;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using UnityEngine;

namespace ClubPenguin.Props
{
	public class StorePropCMD
	{
		private PropUser propUser;

		private bool destroyInstance;

		private Action<bool> onCompleteHandler;

		private bool isPropLocalOwners;

		private bool isResettingControls;

		public StorePropCMD(PropUser propUser, bool destroyInstance, Action<bool> onCompleteHandler = null)
		{
			this.propUser = propUser;
			this.destroyInstance = destroyInstance;
			this.onCompleteHandler = onCompleteHandler;
		}

		public void Execute()
		{
			if (propUser.Prop == null && onCompleteHandler != null)
			{
				onCompleteHandler(false);
			}
			isPropLocalOwners = propUser.Prop.IsOwnerLocalPlayer;
			isResettingControls = (propUser.Prop.IsOwnerLocalPlayer && propUser.Prop.PropDef.IsControlsResetOnStore);
			propUser.EPropRemoved += onPropRemoved;
			propUser.EPropUserEnteredIdle += onPropUserEnteredIdle;
			propUser.StoreProp();
			if (isResettingControls)
			{
				disableInputButtons();
			}
		}

		private void onPropRemoved(Prop prop)
		{
			propUser.EPropRemoved -= onPropRemoved;
			if (destroyInstance)
			{
				UnityEngine.Object.Destroy(prop.gameObject);
			}
			if (isResettingControls)
			{
				enableInputButtons();
			}
		}

		private void onPropUserEnteredIdle()
		{
			propUser.EPropUserEnteredIdle -= onPropUserEnteredIdle;
			if (onCompleteHandler != null)
			{
				onCompleteHandler(isPropLocalOwners);
			}
			if (isResettingControls)
			{
				enableInputButtons();
			}
		}

		private void disableInputButtons()
		{
			Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.DisableUIElement("ControlsButton2"));
			Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.DisableUIElement("ActionButton"));
			Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.DisableUIElement("XButton"));
		}

		private void enableInputButtons()
		{
			Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.EnableUIElement("ControlsButton2"));
			Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.EnableUIElement("XButton"));
			Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.EnableUIElement("ActionButton"));
		}
	}
}
