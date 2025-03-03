// (c) Copyright HutongGames, LLC 2010-2016. All rights reserved.

using UnityEngine;
using HutongGames.PlayMaker.Actions;

namespace HutongGames.PlayMakerEditor
{
    [CustomActionEditor(typeof(GetAnimatorIsLayerInTransition))]
    public class GetAnimatorIsLayerInTransitionActionEditor : OnAnimatorUpdateActionEditorBase
    {

        public override bool OnGUI()
        {
            EditField("gameObject");
            EditField("layerIndex");
            EditField("isInTransition");
            EditField("isInTransitionEvent");
            EditField("isNotInTransitionEvent");

            bool changed = EditEveryFrameField();

            return GUI.changed || changed;
        }

    }
}
