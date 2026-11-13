using ClubPenguin.Core;
using ClubPenguin.Locomotion;
using ClubPenguin.Net.Domain;
using System.Collections.Generic;
using UnityEngine;

public class LocomotionStateSwitch : Switch
{
	public List<LocomotionState> states = new List<LocomotionState>();

	private LocomotionEventBroadcaster locoBroadcaster;

	public void Start()
	{
		GameObject localPlayerGameObject = ClubPenguin.SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject;
		if (!localPlayerGameObject.IsDestroyed())
		{
			locoBroadcaster = localPlayerGameObject.GetComponent<LocomotionEventBroadcaster>();
			if (locoBroadcaster != null)
			{
				locoBroadcaster.OnControllerChangedEvent += onControllerChanged;
				Change(states.Contains(GetLocomotionState(localPlayerGameObject)));
			}
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
		Change(states.Contains(FromController(newController)));
	}

	public static LocomotionState GetLocomotionState(GameObject penguin)
	{
		LocomotionTracker component = penguin.GetComponent<LocomotionTracker>();
		if (component == null)
		{
			return LocomotionState.Default;
		}
		return FromController(component.GetCurrentController());
	}

	public static LocomotionState FromController(LocomotionController controller)
	{
		if (controller is RaceController)
		{
			return LocomotionState.Racing;
		}
		if (controller is SlideController)
		{
			return LocomotionState.Slide;
		}
		if (controller is SitController)
		{
			return LocomotionState.Sitting;
		}
		if (controller is ZiplineController)
		{
			return LocomotionState.Zipline;
		}
		return LocomotionState.Default;
	}

	public override object GetSwitchParameters()
	{
		return states;
	}

	public override string GetSwitchType()
	{
		return "locomotionState";
	}
}
