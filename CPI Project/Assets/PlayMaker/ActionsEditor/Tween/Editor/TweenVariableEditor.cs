// (c) Copyright HutongGames, LLC. All rights reserved.
// See also: EasingFunctionLicense.txt

using System;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using HutongGames.PlayMaker.TweenEnums;
using UnityEditor;

namespace HutongGames.PlayMakerEditor
{
    [CustomActionEditor(typeof(PlayMaker.Actions.TweenFloat))]
    public class TweenFloatEditor : TweenVariableEditor<FsmFloat> {}

    [CustomActionEditor(typeof(PlayMaker.Actions.TweenInt))]
    public class TweenIntEditor : TweenVariableEditor<FsmInt> {}

    [CustomActionEditor(typeof(PlayMaker.Actions.TweenRect))]
    public class TweenRectEditor : TweenVariableEditor<FsmRect> {}

    [CustomActionEditor(typeof(PlayMaker.Actions.TweenVector2))]
    public class TweenVector2Editor : TweenVariableEditor<FsmVector2> {}

    [CustomActionEditor(typeof(PlayMaker.Actions.TweenVector3))]
    public class TweenVector3Editor : TweenVariableEditor<FsmVector3> {}

	public abstract class TweenVariableEditor<T> : TweenEditorBase where T: NamedVariable
	{
	    protected virtual string offsetLabel { get { return "Offset";}}
	    protected virtual string valueLabel { get { return "Value";}}

        protected TweenVariableBase<T> tweenAction;

        public override void OnEnable()
        {
            base.OnEnable();

            tweenAction = target as TweenVariableBase<T>;
        }

        public override bool OnGUI()
        {
            EditorGUI.BeginChangeCheck();

            EditField("variable");

            EditField("fromOption");
            DoTargetValueGUI(tweenAction.fromOption, "fromValue");

            EditField("toOption");
            DoTargetValueGUI(tweenAction.toOption, "toValue");

            DoEasingUI();

            return EditorGUI.EndChangeCheck();
        }

        protected void DoTargetValueGUI(TargetValueOptions option, string valueFieldName)
        {
            switch (option)
            {
                case TargetValueOptions.CurrentValue:
                    break;
                case TargetValueOptions.Offset:
                    EditField(valueFieldName, offsetLabel);
                    break;
                case TargetValueOptions.Value:
                    EditField(valueFieldName, valueLabel);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

    }
}