// (c) Copyright HutongGames, LLC 2010-2016. All rights reserved.

using UnityEngine;
using HutongGames.PlayMaker.Actions;

namespace HutongGames.PlayMakerEditor
{
    [CustomActionEditor(typeof(GetAnimatorBool))]
    public class GetAnimatorBoolActionEditor : OnAnimatorUpdateActionEditorBase
    {

        public override bool OnGUI()
        {
            EditField("gameObject");
            EditField("parameter");
            EditField("result");

            bool changed = EditEveryFrameField();

            return GUI.changed || changed;
        }

    }
}
