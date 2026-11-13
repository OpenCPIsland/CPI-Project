// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Physics)]
    [Tooltip("Controls whether physics affects the game object. See unity docs: <a href=\"http://unity3d.com/support/documentation/ScriptReference/Rigidbody-isKinematic.html\">IsKinematic</a>.")]
    public class SetIsKinematic : ComponentAction<Rigidbody>
	{
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody))]
        [Tooltip("The Game Object to set.")]
        public FsmOwnerDefault gameObject;

		[RequiredField]
        [Tooltip("Set is kinematic true/false.")]
        public FsmBool isKinematic;

		public override void Reset()
		{
			gameObject = null;
			isKinematic = false;
		}

		public override void OnEnter()
		{
			DoSetIsKinematic();
			Finish();
		}

		void DoSetIsKinematic()
		{
			var go = Fsm.GetOwnerDefaultTarget(gameObject);
		    if (UpdateCache(go))
		    {
		        rigidbody.isKinematic = isKinematic.Value;
		    }
		}
	}
}

