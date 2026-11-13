// (c) Copyright HutongGames, LLC. All rights reserved.

using System;
using UnityEngine;
using Random = UnityEngine.Random;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Vector2)]
	[Tooltip("Sets a Vector2 Variable to a random value.")]
	public class Vector2RandomValue : FsmStateAction
	{
        private static bool showPreview;

        public enum Option {Circle, Rectangle, InArc, AtAngles }

        [PreviewField("DrawPreview")]
        [Tooltip("Controls the distribution of the random Vector2 values.")]
        public Option shape;

        [Tooltip("The minimum length for the random Vector2 value.")]
		public FsmFloat minLength;		

        [Tooltip("The maximum length for the random Vector2 value.")]
		public FsmFloat maxLength;

        [Tooltip("Context sensitive parameter. Depends on the Shape.")]
        public FsmFloat floatParam1;
        [Tooltip("Context sensitive parameter. Depends on the Shape.")]
        public FsmFloat floatParam2;

        [Tooltip("Scale the vector in Y (e.g., to squash a circle into an oval)")]
        public FsmFloat yScale;
		
        [RequiredField]
		[UIHint(UIHint.Variable)]
        [Tooltip("Store the result in a Vector2 variable.")]
		public FsmVector2 storeResult;

        // temp working variable
        private Vector2 v2;

		public override void Reset()
        {
            shape = Option.Circle;
			minLength = 0;
			maxLength = 1;
            floatParam1 = null;
            floatParam2 = null;
            yScale = 1;
			storeResult = null;
        }

		public override void OnEnter()
        {
            DoRandomVector2();

            storeResult.Value = v2;
			
			Finish();
		}

        private void DoRandomVector2()
        {
            // note y scale is applied after shape specific calculation
            // this allows any shape to be stretched

            switch (shape)
            {
                case Option.Circle:
                    
                    v2 = Random.insideUnitCircle.normalized * Random.Range(minLength.Value, maxLength.Value);
                    
                    break;

                case Option.Rectangle:
                    
                    var min = minLength.Value;
                    var max = maxLength.Value;
                    
                    v2.x = Random.Range(min, max);
                    if (Random.Range(0, 100) < 50) v2.x = -v2.x;
                    
                    v2.y = Random.Range(min, max);
                    if (Random.Range(0, 100) < 50) v2.y = -v2.y;

                    break;

                case Option.InArc:

                    var angleInArc = Mathf.Deg2Rad * Random.Range(floatParam1.Value, floatParam2.Value);
                    var length = Random.Range(minLength.Value, maxLength.Value);

                    v2.x = Mathf.Cos(angleInArc) * length;
                    v2.y = Mathf.Sin(angleInArc) * length;

                    break;
                
                case Option.AtAngles:

                    var angleStep = (int) floatParam1.Value;

                    var numAngles = 360 / angleStep;
                    var angle = Random.Range(0, numAngles);
                    var radians = Mathf.Deg2Rad * angle * angleStep;
                    length = Random.Range(minLength.Value, maxLength.Value);

                    v2.x = Mathf.Cos(radians) * length;
                    v2.y = Mathf.Sin(radians) * length;

                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }

            v2.y *= yScale.Value;
        }


#if UNITY_EDITOR

        private const string showPreviewPrefsKey = "PlayMaker.ShowRandomVector2Preview";

        private static Color previewColor;

        public override void InitEditor(Fsm fsmOwner)
        {
            Fsm = fsmOwner; // not serialized to avoid recursive serialization errors
            showPreview = EditorPrefs.GetBool(showPreviewPrefsKey, true);

            previewColor = Color.white;
            previewColor.a = 0.5f;
        }

        [SettingsMenuItem("Show Preview")]
        public static void TogglePreview()
        {
            showPreview = !showPreview;
            EditorPrefs.SetBool(showPreviewPrefsKey, showPreview);
        }

        [SettingsMenuItemState("Show Preview")]
        public static bool GetPreviewState()
        {
            return showPreview;
        }

        public void DrawPreview(object fieldValue)
        {
            if (!showPreview) return;

            var area = ActionHelpers.GetControlPreviewRect(100f);            
            var axisCenter = ActionHelpers.DrawAxisXY(area);
            
            Handles.color = previewColor;

            // init min/max sizes for drawing

            var max = 40f;
            var min = 0f;

            if (maxLength.Value > 0)
            {
                min = max * (minLength.Value / maxLength.Value);
            }

            // setup x/y scales to fit in area

            var scaleX = 1f;
            var scaleY = yScale.Value;
            if (Mathf.Abs(scaleY) > 1f)
            {
                scaleX = 1f / scaleY;
                scaleY = 1f;
            }

            switch (shape)
            {
                case Option.Circle:

                    ActionHelpers.DrawOval(axisCenter, max * scaleX, max * scaleY);
                    ActionHelpers.DrawOval(axisCenter, min * scaleX, min * scaleY);

                    break;
                
                case Option.Rectangle:
 
                    ActionHelpers.DrawRect(axisCenter, max * scaleX, max * scaleY);
                    ActionHelpers.DrawRect(axisCenter, min * scaleX, min * scaleY);
 
                    break;
                
                case Option.InArc:

                    var fromAngle = floatParam1.Value;
                    var toAngle = floatParam2.Value;

                    ActionHelpers.DrawArc(axisCenter, fromAngle, toAngle, max * scaleX, max * scaleY);
                    ActionHelpers.DrawArc(axisCenter, fromAngle, toAngle, min * scaleX, min * scaleY);

                    ActionHelpers.DrawSpoke(axisCenter, fromAngle, min, max, scaleX, scaleY);
                    ActionHelpers.DrawSpoke(axisCenter, toAngle, min, max, scaleX, scaleY);


                    break;
                
                case Option.AtAngles:

                    var angleStep = floatParam1.Value;
                    if (angleStep < 1) angleStep = 1;
                    
                    // make sure we draw at least a dot (should be a better way?!)
                    if (max - min < 1.4f) min = max - 1.4f; 

                    for (float angle = 0; angle < 360; angle += angleStep)
                    {
                        ActionHelpers.DrawSpoke(axisCenter, angle, min, max, scaleX, scaleY);
                    }

                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Handles.color = Color.white;
        }

        public override string AutoName()
        {
            switch (shape)
            {
                case Option.Circle:
                    return ActionHelpers.AutoName("Random Vector2 in Circle", minLength, maxLength);
                case Option.Rectangle:
                    return ActionHelpers.AutoName("Random Vector2 in Rectangle", minLength, maxLength);
                case Option.InArc:
                    return ActionHelpers.AutoName("Random Vector2 in Arc", minLength, maxLength);
                case Option.AtAngles:
                    return ActionHelpers.AutoName("Random Vector2 at Angles", floatParam1, minLength, maxLength);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
#endif
	}
}