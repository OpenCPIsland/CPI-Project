// (c) Copyright HutongGames, LLC 2020. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Activates a GameObject and de-activates other GameObjects at the same level of the hierarchy. " +
             "E.g, a single UI Screen, a single accessory etc. " +
             "This action is very helpful if you often organize GameObject hierarchies in this way. " +
             "\nNOTE: The targeted GameObject should have a parent. This action will not work on GameObjects at the scene root.")]
	public class ActivateSolo : FsmStateAction
	{
		[RequiredField]
        [Tooltip("The GameObject to activate.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("Re-activate if already active. This means deactivating the target GameObject then activating it again. " +
                 "This will reset FSMs on that GameObject.")]
        public FsmBool allowReactivate;

        // Keep track if we just reactivated
        // to avoid infinite loops
        private int activatedFrame = -1;

        public override void Reset()
		{
			gameObject = null;
            allowReactivate = new FsmBool {Value = true};
		}

		public override void OnEnter()
		{
			DoActivateSolo();

            Finish();			
		}

		void DoActivateSolo()
		{
			var go = Fsm.GetOwnerDefaultTarget(gameObject);
            if (go == null || go.transform.parent == null) return;

            var goTransform = go.transform;
            var parent = go.transform.parent.transform;

            foreach (Transform child in parent)
            {
	            if (child != goTransform)
	            {
		            child.gameObject.SetActive(false);
	            }
            }

            if (allowReactivate.Value && Time.frameCount != activatedFrame)
            {
	            goTransform.gameObject.SetActive(false);
	            activatedFrame = Time.frameCount;
            }
            
            go.SetActive(true);
		}

#if UNITY_EDITOR
        public override string AutoName()
        {
            return ActionHelpers.AutoName(this, Fsm, gameObject);
        }
#endif
    }
}