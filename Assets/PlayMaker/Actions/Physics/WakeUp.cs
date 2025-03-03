// (c) Copyright HutongGames, LLC 2010-2020. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Physics)]
	[Tooltip("Forces a Game Object's Rigid Body to wake up. " +
             "See Unity Docs: <a href=\"http://unity3d.com/support/documentation/ScriptReference/Rigidbody.WakeUp.html\">Rigidbody.WakeUp</a>.")]
    [SeeAlso("<a href =\"http://unity3d.com/support/documentation/ScriptReference/Rigidbody.WakeUp.html\">Rigidbody.WakeUp</a>")]
    public class WakeUp : ComponentAction<Rigidbody>
	{
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody))]
        [Tooltip("The Game Object to wake up.")]
		public FsmOwnerDefault gameObject;

		public override void Reset()
		{
			gameObject = null;
		}

		public override void OnEnter()
		{
			DoWakeUp();
			Finish();
		}

		private void DoWakeUp()
		{
			var go = gameObject.OwnerOption == OwnerDefaultOption.UseOwner ? Owner : gameObject.GameObject.Value;
		    if (UpdateCache(go))
		    {
		        rigidbody.WakeUp();
		    }
		}
	}
}