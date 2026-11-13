// (c) Copyright HutongGames, LLC 2010-2020. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Physics)]
	[Tooltip("Rigid bodies start sleeping when they come to rest. " +
             "This action wakes up all rigid bodies in the scene. " +
             "E.g., if you Set Gravity and want objects at rest to respond." +
             "See Unity Docs: <a href=\"http://unity3d.com/support/documentation/ScriptReference/Rigidbody.WakeUp.html\">Rigidbody.WakeUp</a>.")]
    [SeeAlso("<a href =\"http://unity3d.com/support/documentation/ScriptReference/Rigidbody.WakeUp.html\">Rigidbody.WakeUp</a>")]
	public class WakeAllRigidBodies : FsmStateAction
	{
        [Tooltip("Do it every frame - use with caution! Sleeping is an important physics optimization!")]
		public bool everyFrame;
		
		private Rigidbody[] bodies;
		
		public override void Reset()
		{
			everyFrame = false;
		}

		public override void OnEnter()
		{
            DoWakeAll();
			
			if (!everyFrame)
				Finish();		
		}
		
		public override void OnUpdate()
		{
			DoWakeAll();
		}
		
		void DoWakeAll()
		{
#if UNITY_2022_3_OR_NEWER
			bodies = Object.FindObjectsByType<Rigidbody>(FindObjectsSortMode.None);
#else
            bodies = Object.FindObjectsOfType(typeof(Rigidbody)) as Rigidbody[];
#endif
            
			if (bodies != null)
			{
				foreach (var body in bodies)
				{
					body.WakeUp();
				}
			}
		}
	}
}