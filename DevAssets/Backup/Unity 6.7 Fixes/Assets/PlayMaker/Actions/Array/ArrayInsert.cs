// (c) Copyright HutongGames, LLC 2010-2014. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Array)]
    [Tooltip("Insert an item into an Array at the specified index.")]
    public class ArrayInsert : FsmStateAction
    {
        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("The Array Variable to use.")]
        public FsmArray array;

        [RequiredField]
        [MatchElementType("array")]
        [Tooltip("Item to add.")]
        public FsmVar value;

        [Tooltip("The index to insert at.\n0 = first, 1 = second...")]
        public FsmInt atIndex;

        public override void Reset()
        {
            array = null;
            value = null;
            atIndex = null;
        }

        public override void OnEnter()
        {
            DoInsertValue();
            Finish();
        }

        private void DoInsertValue()
        {
            value.UpdateValue();
            array.InsertItem(value.GetValue(), atIndex.Value);
        }


#if UNITY_EDITOR
        public override string AutoName()
        {
            return ActionHelpers.AutoName(this, array);
            //return "Insert: " +  ActionHelpers.GetValueLabel(array);
        }
#endif

    }

}

