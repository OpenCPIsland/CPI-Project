using ClubPenguin.Core;
using ClubPenguin.Locomotion;
using System;

namespace ClubPenguin.Switches
{
	public class LocomotionControllerSwitch : Switch
	{
		public bool Run;

		public bool Sit;

		public bool Tube;

		public bool Swim;

		public bool Zipline;

		private LocomotionEventBroadcaster locoBroadcaster;

		public override string GetSwitchType()
		{
			throw new NotImplementedException();
		}

		public override object GetSwitchParameters()
		{
			throw new NotImplementedException();
		}

		public void Start()
		{
			locoBroadcaster = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject.GetComponent<LocomotionEventBroadcaster>();
			if (locoBroadcaster != null)
			{
				locoBroadcaster.OnControllerChangedEvent += onControllerChanged;
			}
		}

		public void OnDestroy()
		{
			if (locoBroadcaster != null)
			{
				locoBroadcaster.OnControllerChangedEvent -= onControllerChanged;
			}
		}

		private void onControllerChanged(LocomotionController newController)
		{
			if (Run && newController is RunController)
			{
				Change(true);
			}
			else if (Sit && newController is SitController)
			{
				Change(true);
			}
			else if (Tube && newController is SlideController)
			{
				Change(true);
			}
			else if (Swim && newController is SwimController)
			{
				Change(true);
			}
			else if (Zipline && newController is ZiplineController)
			{
				Change(true);
			}
			else
			{
				Change(false);
			}
		}
	}
}
