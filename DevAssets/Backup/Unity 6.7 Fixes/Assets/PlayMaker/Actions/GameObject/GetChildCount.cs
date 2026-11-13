// (c) Copyright HutongGames, LLC. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.GameObject)]
    [Tooltip("Gets the number of children that a GameObject has.")]
    public class GetChildCount : FsmStateAction
    {
        [RequiredField]
        [Tooltip("The GameObject to test.")]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("Store the number of children in an int variable.")]
        public FsmInt storeResult;

        [Tooltip("Repeat every frame. Useful if you're waiting for a specific count.")]
        public bool everyFrame;

        public override void Reset()
        {
            gameObject = null;
            storeResult = null;
            everyFrame = false;
        }

        public override void OnEnter()
        {
            DoGetChildCount();

            if (!everyFrame)
            {
                Finish();
            }
        }

        public override void OnUpdate()
        {
            DoGetChildCount();
        }

        void DoGetChildCount()
        {
            var go = Fsm.GetOwnerDefaultTarget(gameObject);
            if (go == null) return;

            storeResult.Value = go.transform.childCount;
        }

#if UNITY_EDITOR
        public override string AutoName()
        {
            return ActionHelpers.AutoName(this, storeResult);
        }
#endif
    }
}