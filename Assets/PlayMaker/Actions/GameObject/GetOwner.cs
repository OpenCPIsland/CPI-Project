// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Gets the Game Object that owns the FSM and stores it in a Game Object variable.")]
	public class GetOwner : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
        [Tooltip("Store the Owner in a Game Object variable.")]
		public FsmGameObject storeGameObject;

		public override void Reset()
		{
			storeGameObject = null;
		}

		public override void OnEnter()
		{
			storeGameObject.Value = Owner;
			
			Finish();
		}
	}
}