using ClubPenguin.UI;
using UnityEngine;

namespace ClubPenguin.Input
{
	[RequireComponent(typeof(BackButtonController))]
	public class BackControllerInputHandler : InputMapHandler<BackControllerInputMap.Result>
	{
		private BackButtonController backController;

		protected override void Awake()
		{
			backController = GetComponent<BackButtonController>();
			base.Awake();
		}

		protected override void onHandle(BackControllerInputMap.Result inputResult)
		{
			if (inputResult.Back.WasJustReleased)
			{
				backController.Execute();
			}
		}

		protected override void onReset()
		{
		}
	}
}
