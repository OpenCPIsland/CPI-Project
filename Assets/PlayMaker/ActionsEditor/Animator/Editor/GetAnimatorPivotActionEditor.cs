// (c) Copyright HutongGames, LLC 2010-2016. All rights reserved.

using UnityEngine;
using HutongGames.PlayMaker.Actions;

namespace HutongGames.PlayMakerEditor
{
    [CustomActionEditor(typeof(GetAnimatorPivot))]
    public class GetAnimatorPivotActionEditor : OnAnimatorUpdateActionEditorBase
    {

        public override bool OnGUI()
        {
            EditField("gameObject");
            EditField("pivotWeight");
            EditField("pivotPosition");

            bool changed = EditEveryFrameField();

            return GUI.changed || changed;
        }

    }
}
