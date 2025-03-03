// (c) Copyright HutongGames, all rights reserved.
// See also: EasingFunctionLicense.txt

using System;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using HutongGames.PlayMaker.TweenEnums;
using UnityEditor;

namespace HutongGames.PlayMakerEditor
{
	public abstract class TweenPropertyEditor<T> : TweenEditorBase where T: NamedVariable
	{
	    protected virtual string offsetLabel { get { return "Offset";}}
	    protected virtual string valueLabel { get { return "Value";}}

        protected TweenPropertyBase<T> tweenAction;

        public override void OnEnable()
        {
            base.OnEnable();

            tweenAction = target as TweenPropertyBase<T>;
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