
using System;
using HutongGames.PlayMaker.Actions;
using UnityEditor;

namespace HutongGames.PlayMakerEditor
{
    [CustomActionEditor(typeof(HutongGames.PlayMaker.Actions.Vector2RandomValue))]
    public class Vector2RandomValueEditor : CustomActionEditor
    {
        private Vector2RandomValue action;

        public override void OnEnable()
        {
            action = target as Vector2RandomValue;
        }

        public override bool OnGUI()
        {
            EditorGUI.BeginChangeCheck();

            EditShape();

            switch (action.shape)
            {
                case Vector2RandomValue.Option.Circle:
                    
                    EditField("minLength");
                    EditField("maxLength");

                    break;

                case Vector2RandomValue.Option.Rectangle:

                    EditField("minLength");
                    EditField("maxLength");

                    break;

                case Vector2RandomValue.Option.InArc:      
                    
                    EditField("floatParam1", "Min Angle");
                    EditField("floatParam2", "Max Angle");
                    EditField("minLength");
                    EditField("maxLength");

                    break;

                case Vector2RandomValue.Option.AtAngles:
                   
                    //Hint("Example, 90 selects randomly between 0, 90, 180, and 270 degrees. 45 selects randomly between 8 directions (every 45 degrees).");
                    
                    EditField("floatParam1", "Angle Step");
                    EditField("minLength");
                    EditField("maxLength");
                    
                    if (action.floatParam1.Value < 0) action.floatParam1.Value = 0f;
                    
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (action.minLength.Value < 0) action.minLength.Value = 0f;
            if (action.maxLength.Value < 0) action.maxLength.Value = 0f;

            if (action.minLength.Value > action.maxLength.Value)
            {
                action.minLength.Value = action.maxLength.Value;
            }

            if (action.maxLength.Value < action.minLength.Value)
            {
                action.minLength.Value = action.maxLength.Value;
            }

            EditField("yScale");

            EditField("storeResult");

            return EditorGUI.EndChangeCheck();
        }

        private void EditShape()
        {
            var shape = action.shape;
            EditField("shape");
            if (shape == action.shape) return;

            // set sensible defaults

            switch (action.shape)
            {
                case Vector2RandomValue.Option.Circle:
                    action.minLength = 0;
                    action.maxLength = 1;
                    break;
                case Vector2RandomValue.Option.Rectangle:
                    action.minLength = 0f;
                    action.maxLength = 1f;
                    break;
                case Vector2RandomValue.Option.InArc:
                    action.minLength = 0f;
                    action.maxLength = 1f;
                    action.floatParam1 = -45f;
                    action.floatParam2 = 45f;
                    break;
                case Vector2RandomValue.Option.AtAngles:
                    action.floatParam1 = 45f;
                    action.minLength = 0f;
                    action.maxLength = 1f;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            action.yScale = 1f;
        }
    }
}