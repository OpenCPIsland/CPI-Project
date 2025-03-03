// (c) Copyright HutongGames, LLC 2010-2016. All rights reserved.

using UnityEngine;
using HutongGames.PlayMaker.Actions;

namespace HutongGames.PlayMakerEditor
{
    [CustomActionEditor(typeof(GetAnimatorCurrentTransitionInfoIsName))]
    public class GetAnimatorCurrentTransitionInfoIsNameActionEditor : OnAnimatorUpdateActionEditorBase
    {

        public override bool OnGUI()
        {
            EditField("gameObject");
            EditField("layerIndex");
            EditField("name");

            EditField("nameMatch");
            EditField("nameMatchEvent");
            EditField("nameDoNotMatchEvent");

            bool changed = EditEveryFrameField();

            return GUI.changed || changed;
        }

    }
}
