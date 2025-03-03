using ClubPenguin;
using ClubPenguin.Locomotion;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Locomotion")]
	public class WaitForLocomotionStateAction : FsmStateAction
	{
		public enum LocomotionState
		{
			Run,
			Sit,
			Slide,
			Swim,
			Zipline
		}

		public LocomotionState StateToCheck;

		public FsmEvent StateMatchesEvent;

		private GameObject playerObject;

		public override void OnEnter()
		{
			playerObject = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject;
			if (playerObject != null)
			{
				bool flag = false;
				switch (StateToCheck)
				{
				case LocomotionState.Run:
					flag = (playerObject.GetComponent<RunController>() != null && playerObject.GetComponent<RunController>().isActiveAndEnabled);
					break;
				case LocomotionState.Sit:
					flag = (playerObject.GetComponent<SitController>() != null && playerObject.GetComponent<SitController>().isActiveAndEnabled);
					break;
				case LocomotionState.Slide:
					flag = (playerObject.GetComponent<SlideController>() != null && playerObject.GetComponent<SlideController>().isActiveAndEnabled);
					break;
				case LocomotionState.Swim:
					flag = (playerObject.GetComponent<SwimController>() != null && playerObject.GetComponent<SwimController>().isActiveAndEnabled);
					break;
				case LocomotionState.Zipline:
					flag = (playerObject.GetComponent<ZiplineController>() != null && playerObject.GetComponent<ZiplineController>().isActiveAndEnabled);
					break;
				}
				if (flag)
				{
					base.Fsm.Event(StateMatchesEvent);
				}
				else
				{
					playerObject.GetComponent<LocomotionEventBroadcaster>().OnControllerChangedEvent += onControllerChanged;
				}
			}
			else
			{
				Finish();
			}
		}

		private void onControllerChanged(LocomotionController newController)
		{
			bool flag = false;
			switch (StateToCheck)
			{
			case LocomotionState.Run:
				flag = (newController is RunController);
				break;
			case LocomotionState.Sit:
				flag = (newController is SitController);
				break;
			case LocomotionState.Slide:
				flag = (newController is SlideController);
				break;
			case LocomotionState.Swim:
				flag = (newController is SwimController);
				break;
			case LocomotionState.Zipline:
				flag = (newController is ZiplineController);
				break;
			}
			if (flag)
			{
				playerObject.GetComponent<LocomotionEventBroadcaster>().OnControllerChangedEvent -= onControllerChanged;
				base.Fsm.Event(StateMatchesEvent);
			}
		}
	}
}
