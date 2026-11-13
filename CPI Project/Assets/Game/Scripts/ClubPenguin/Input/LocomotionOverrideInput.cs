using ClubPenguin.Core;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.Input
{
	public class LocomotionOverrideInput : LocomotionInput
	{
		[SerializeField]
		private Vector2 OverrideDirection;

		[SerializeField]
		private bool DirectionOverriden;

		public override void Initialize(KeyCodeRemapper keyCodeRemapper)
		{
			Service.Get<EventDispatcher>().AddListener<InputEvents.OverrideMoveEvent>(onOverrideMovement);
			base.Initialize(keyCodeRemapper);
		}

		private bool onOverrideMovement(InputEvents.OverrideMoveEvent evt)
		{
			OverrideDirection = evt.Direction;
			DirectionOverriden = true;
			return false;
		}

		protected override bool process(int filter)
		{
			bool result = false;
			if (DirectionOverriden)
			{
				inputEvent.Direction = OverrideDirection;
				result = true;
				DirectionOverriden = false;
			}
			return result;
		}
	}
}
