using Disney.Kelowna.Common.SEDFSM;
using UnityEngine;

namespace ClubPenguin.Input
{
	public class NavBarSecondaryInputHandler : InputMapHandler<NavBarSecondaryInputMap.Result>
	{
		[SerializeField]
		private string target = string.Empty;

		[SerializeField]
		private string targetEvent = string.Empty;

		private new void OnValidate()
		{
		}

		protected override void onHandle(NavBarSecondaryInputMap.Result inputResult)
		{
			if (inputResult.Close.WasJustReleased || inputResult.Locomotion.Direction.sqrMagnitude > float.Epsilon)
			{
				StateMachineContext componentInParent = GetComponentInParent<StateMachineContext>();
				componentInParent.SendEvent(new ExternalEvent(target, targetEvent));
			}
		}

		protected override void onReset()
		{
		}
	}
}
