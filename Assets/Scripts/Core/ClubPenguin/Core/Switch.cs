using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
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
