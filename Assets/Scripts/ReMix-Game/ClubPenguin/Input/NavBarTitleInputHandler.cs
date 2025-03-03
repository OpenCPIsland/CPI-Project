using Disney.Kelowna.Common.SEDFSM;
using UnityEngine;

namespace ClubPenguin.Input
{
	public class NavBarTitleInputHandler : InputMapHandler<NavBarTitleInputMap.Result>
	{
		[SerializeField]
		[Header("Buttons")]
		private InputMappedButton back = null;

		[SerializeField]
		private InputMappedButton exit = null;

		[SerializeField]
		[Header("State Machine")]
		private string target = string.Empty;

		[SerializeField]
		private string targetEvent = string.Empty;

		private new void OnValidate()
		{
		}

		protected override void onHandle(NavBarTitleInputMap.Result inputResult)
		{
			back.HandleMappedInput(inputResult.Back);
			exit.HandleMappedInput(inputResult.Exit);
			if (inputResult.Locomotion.Direction.sqrMagnitude > float.Epsilon)
			{
				StateMachineContext componentInParent = GetComponentInParent<StateMachineContext>();
				componentInParent.SendEvent(new ExternalEvent(target, targetEvent));
			}
		}

		protected override void onReset()
		{
			back.HandleMappedInput();
			exit.HandleMappedInput();
		}
	}
}
