// (c) Copyright HutongGames, LLC 2010-2016. All rights reserved.

using UnityEngine;
using HutongGames.PlayMaker.Actions;

namespace HutongGames.PlayMakerEditor
{
    [CustomActionEditor(typeof(SetAnimatorFloat))]
    public class SetAnimatorFloatActionEditor : OnAnimatorUpdateActionEditorBase
    {

        public override bool OnGUI()
        {
            EditField("gameObject");
            EditField("parameter");
            EditField("Value");
            EditField("dampTime");

            bool changed = EditEveryFrameField();

            return GUI.changed || changed;
        }

    }
}
