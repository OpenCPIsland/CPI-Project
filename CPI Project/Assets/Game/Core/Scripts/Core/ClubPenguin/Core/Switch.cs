using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using UnityEngine;

namespace ClubPenguin.Core
{
	[DisallowMultipleComponent]
	public abstract class Switch : MonoBehaviour
	{
		private Switch parentSwitch;

		private EventDispatcher dispatcher;

		public bool Latch;

		private bool firstTime = true;

		public bool OnOff
		{
			get;
			private set;
		}

		// Switches inside a group never reach the dispatcher, they pass their
		// state up to the parent and only the root dispatches, so this is the
		// only way to hear the parts on their own
		public event Action<Switch, bool> StateChanged;

		public void Awake()
		{
			if (base.transform.parent != null)
			{
				parentSwitch = base.transform.parent.GetComponent<Switch>();
			}
			dispatcher = Service.Get<EventDispatcher>();
		}

		public abstract string GetSwitchType();

		public abstract object GetSwitchParameters();

		protected virtual void Change(bool onoff)
		{
			onoff |= (Latch & OnOff);
			if (OnOff != onoff || firstTime)
			{
				OnOff = onoff;
				if (parentSwitch != null)
				{
					parentSwitch.Change(onoff);
				}
				else
				{
					dispatcher.DispatchEvent(new SwitchEvents.SwitchChange(base.transform, onoff));
				}
				firstTime = false;
				if (StateChanged != null)
				{
					StateChanged(this, onoff);
				}
			}
		}

		public void OnDrawGizmos()
		{
			if (OnOff)
			{
				Gizmos.DrawIcon(base.transform.position, "Switches/On.png");
			}
			else
			{
				Gizmos.DrawIcon(base.transform.position, "Switches/Off.png");
			}
		}
	}
}
