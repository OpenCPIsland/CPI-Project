using HutongGames.PlayMaker;
using UnityEngine;

namespace ClubPenguin.Adventure
{
	[ActionCategory("GUI")]
	public class GetMousePosition : FsmStateAction
	{
		public FsmVector2 MousePositionVariable;

		public override void OnEnter()
		{
			MousePositionVariable.Value = new Vector2(UnityEngine.Input.mousePosition.x, UnityEngine.Input.mousePosition.y);
			Finish();
		}
	}
}
