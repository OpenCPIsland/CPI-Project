using ClubPenguin.Net.Domain;
using UnityEngine;

namespace ClubPenguin.Core
{
	public static class InputEvents
	{
		public struct MoveEvent
		{
			public readonly Vector2 Direction;

			public MoveEvent(Vector2 direction)
			{
				Direction = direction;
			}
		}

		public struct RotateEvent
		{
			public readonly Vector2 Direction;

			public RotateEvent(Vector2 direction)
			{
				Direction = direction;
			}
		}

		public struct ZoomEvent
		{
			public readonly float Factor;

			public ZoomEvent(float _factor)
			{
				Factor = _factor;
			}
		}

		public struct SwipeEvent
		{
			public readonly float Delta;

			public SwipeEvent(float _delta)
			{
				Delta = _delta;
			}
		}

		public enum Actions
		{
			None = -1,
			Jump,
			Interact,
			Snowball,
			Action1,
			Action2,
			Action3,
			Torpedo,
			Cancel
		}

		public struct ActionEvent
		{
			public readonly Actions Action;

			public ActionEvent(Actions _action)
			{
				Action = _action;
			}
		}

		public struct LocomotionStateEvent
		{
			public readonly LocomotionState LocomotionState;

			public LocomotionStateEvent(LocomotionState value)
			{
				LocomotionState = value;
			}
		}

		public enum ChargeActions
		{
			None = -1,
			Snowball
		}

		public struct ChargeActionEvent
		{
			public readonly ChargeActions Action;

			public readonly bool ButtonState;

			public readonly float HoldTime;

			public ChargeActionEvent(ChargeActions _action, bool _buttonState, float _holdTime)
			{
				Action = _action;
				ButtonState = _buttonState;
				HoldTime = _holdTime;
			}
		}

		public enum Switches
		{
			None = -1,
			Tube
		}

		public struct SwitchChangeEvent
		{
			public readonly Switches Switch;

			public readonly bool Value;

			public SwitchChangeEvent(Switches _switch, bool _value)
			{
				Switch = _switch;
				Value = _value;
			}
		}

		public enum Cycles
		{
			None = -1,
			Walk,
			Run
		}

		public struct CycleChangeEvent
		{
			public readonly Cycles Cycle;

			public CycleChangeEvent(Cycles _cycle)
			{
				Cycle = _cycle;
			}
		}

		public struct ActionEnabledEvent
		{
			public readonly Actions Action;

			public readonly bool Enabled;

			public ActionEnabledEvent(Actions _action, bool _enabled)
			{
				Action = _action;
				Enabled = _enabled;
			}
		}

		public struct OverrideMoveEvent
		{
			public readonly Vector2 Direction;

			public OverrideMoveEvent(Vector2 direction)
			{
				Direction = direction;
			}
		}
	}
}
