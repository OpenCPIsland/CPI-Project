// (c) Copyright HutongGames, all rights reserved.
// See also: EasingFunctionLicense.txt

using UnityEditor;

namespace HutongGames.PlayMakerEditor
{
    [CustomActionEditor(typeof(PlayMaker.Actions.TweenUiSize))]
    public class TweenUiSizeEditor : TweenEditorBase
    {
        public override bool OnGUI()
        {
            EditorGUI.BeginChangeCheck();

            EditField("gameObject");
            EditField("tweenDirection", "Tween");
            EditField("targetSize", "Size");

            DoEasingUI();

            return EditorGUI.EndChangeCheck();
        }
    }
}