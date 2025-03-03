using ClubPenguin.Core;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using UnityEngine;

namespace ClubPenguin.Props
{
	[RequireComponent(typeof(Prop))]
	[DisallowMultipleComponent]
	public class PropActionButtonDelegates : MonoBehaviour
	{
		public event Action<InputEvents.Actions> ActionButtonPressed;

		public void Awake()
		{
			Service.Get<EventDispatcher>().AddListener<InputEvents.ActionEvent>(onActionEvent);
		}

		private bool onActionEvent(InputEvents.ActionEvent evt)
		{
			this.ActionButtonPressed(evt.Action);
			return false;
		}
	}
}
