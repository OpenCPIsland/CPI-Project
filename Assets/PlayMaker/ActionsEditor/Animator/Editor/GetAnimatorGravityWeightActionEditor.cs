// (c) Copyright HutongGames, LLC 2010-2016. All rights reserved.

using UnityEngine;
using HutongGames.PlayMaker.Actions;

namespace HutongGames.PlayMakerEditor
{
    [CustomActionEditor(typeof(GetAnimatorGravityWeight))]
    public class GetAnimatorGravityWeightActionEditor : OnAnimatorUpdateActionEditorBase
    {

        public override bool OnGUI()
        {
            EditField("gameObject");
            EditField("gravityWeight");

            bool changed = EditEveryFrameField();

            return GUI.changed || changed;
        }

    }
}
