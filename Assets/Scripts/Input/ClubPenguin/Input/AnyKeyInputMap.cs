using UnityEngine;

namespace ClubPenguin.Input
{
	public class AnyKeyInputMap : InputMap<AnyKeyInputMap.Result>
	{
		public class Result
		{
			public readonly ButtonInputResult AnyKey = new ButtonInputResult();
		}

		public override void AddHandler(InputHandlerCallback<Result> handler)
		{
			mapResult.AnyKey.Reset();
			base.AddHandler(handler);
		}

		protected override bool processInput(ControlScheme controlScheme)
		{
			bool anyKey = UnityEngine.Input.anyKey;
			mapResult.AnyKey.WasJustPressed = (anyKey && !mapResult.AnyKey.IsHeld);
			mapResult.AnyKey.WasJustReleased = (!anyKey && mapResult.AnyKey.IsHeld);
			mapResult.AnyKey.IsHeld = anyKey;
			return mapResult.AnyKey.IsHeld || mapResult.AnyKey.WasJustReleased;
		}
	}
}
