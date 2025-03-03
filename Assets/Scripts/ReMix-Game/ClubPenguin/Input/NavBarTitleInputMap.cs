using System;

namespace ClubPenguin.Input
{
	public class NavBarTitleInputMap : InputMap<NavBarTitleInputMap.Result>
	{
		public class Result
		{
			public readonly LocomotionInputResult Locomotion = new LocomotionInputResult();

			public readonly ButtonInputResult Back = new ButtonInputResult();

			public readonly ButtonInputResult Exit = new ButtonInputResult();
		}

		[NonSerialized]
		public bool DisableLocomotion;

		protected override bool processInput(ControlScheme controlScheme)
		{
			bool flag = controlScheme.Cancel.ProcessInput(mapResult.Back);
			flag = (controlScheme.Back.ProcessInput(mapResult.Exit) || flag);
			return (!DisableLocomotion && controlScheme.Locomotion.ProcessInput(mapResult.Locomotion)) || flag;
		}
	}
}
