using ClubPenguin.UI;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using UnityEngine;

namespace ClubPenguin.Props
{
	public class UsePropCMD
	{
		private PropUser propUser;

		private Vector3 destination;

		private bool destroyInstanceOnRemove;

		private string experienceInstanceId;

		private PropDefinition propDef;

		private System.Action onCompleteHandler;

		public UsePropCMD(PropUser propUser, Vector3 destination, bool destroyInstanceOnRemove, string experienceInstanceId, System.Action onCompleteHandler = null)
		{
			this.propUser = propUser;
			this.destination = destination;
			this.destroyInstanceOnRemove = destroyInstanceOnRemove;
			this.experienceInstanceId = experienceInstanceId;
			this.onCompleteHandler = onCompleteHandler;
		}

		public void Execute()
		{
			if (!(propUser != null) || !(propUser.gameObject != null) || propUser.gameObject.Equals(null))
			{
				return;
			}
			if (propUser.Prop == null)
			{
				if (onCompleteHandler != null)
				{
					onCompleteHandler();
				}
				return;
			}
			propUser.EPropUseCompleted += onPropUseCompleted;
			propUser.EPropRemoved += onPropRemoved;
			propUser.Prop.ExperienceInstanceId = experienceInstanceId;
			propUser.UsePropAtDestination(destination);
			if (propUser.Prop.IsOwnerLocalPlayer)
			{
				disableInputButtons();
			}
		}

		private void disableInputButtons()
		{
			Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.DisableUIElement("ControlsButton2"));
			Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.DisableUIElement("XButton"));
		}

		private void enableInputButtons()
		{
			Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.EnableUIElement("ControlsButton2"));
			Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.EnableUIElement("XButton"));
		}

		private void onPropRemoved(Prop prop)
		{
			if (destroyInstanceOnRemove)
			{
				UnityEngine.Object.Destroy(prop.gameObject);
			}
			onPropUseCompleted(prop);
		}

		private void onPropUseCompleted(Prop prop)
		{
			if (propUser != null && propUser.gameObject != null && !propUser.gameObject.Equals(null))
			{
				propUser.EPropUseCompleted -= onPropUseCompleted;
				propUser.EPropRemoved -= onPropRemoved;
				if (onCompleteHandler != null)
				{
					onCompleteHandler();
				}
				if (prop.IsOwnerLocalPlayer)
				{
					enableInputButtons();
				}
			}
		}
	}
}
