// (c) Copyright HutongGames, LLC. All rights reserved.

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Vector2)]
	[Tooltip("Snap a Vector2 to an angle increment while maintaining length.")]
	public class Vector2SnapToAngle : FsmStateAction
	{
        private static bool showPreview;

		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The vector to snap to an angle.")]
		public FsmVector2 vector2Variable;

        [PreviewField("DrawPreview")]
        [Tooltip("Angle increment to snap to.")]
        public FsmFloat snapAngle;

		[Tooltip("Repeat every frame")]
		public bool everyFrame;

		public override void Reset()
		{
			vector2Variable = null;
            snapAngle = 15f;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoSnapToAngle();
			
			if (!everyFrame)
			{
				Finish();
			}		
		}

		public override void OnUpdate()
		{
			DoSnapToAngle();
		}

        private void DoSnapToAngle()
        {
            var angle = snapAngle.Value;
            if (angle <= 0) return;

            var v2 = vector2Variable.Value;
            var length = v2.magnitude;
            var currentAngle = Mathf.Atan2(v2.y, v2.y);            
            var newAngle = Mathf.Deg2Rad * Mathf.Round(currentAngle / angle) * angle;

            vector2Variable.Value = new Vector2(Mathf.Cos(newAngle) * length, Mathf.Sin(newAngle) * length);
        }

#if UNITY_EDITOR

        private const string showPreviewPrefsKey = "PlayMaker.Vector2SnapToAngle";

        public override void InitEditor(Fsm fsmOwner)
        {
            Fsm = fsmOwner; // not serialized to avoid recursive serialization errors
            showPreview = EditorPrefs.GetBool(showPreviewPrefsKey, true);
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

        private Vector2 start, end;

        public void DrawPreview(object fieldValue)
        {
            if (!showPreview) return;

            var area = ActionHelpers.GetControlPreviewRect(100f);
            
            var previewColor = Color.white;
            previewColor.a = 0.5f;
            Handles.color = previewColor;

            start.x = area.x + area.width * 0.5f;
            start.y = area.y + area.height * 0.5f;

            var max = 40f;

            var angleStep = snapAngle.Value;
            if (angleStep < 1) angleStep = 1;

            for (float angle = 0; angle < 360; angle += angleStep)
            {
                var radians = Mathf.Deg2Rad * angle;
                end.x = start.x + Mathf.Cos(radians) * max;
                end.y = start.y + Mathf.Sin(radians) * max;

                Handles.DrawLine(start, end);
            }

                        ActionHelpers.DrawAxisXY(area);

            Handles.color = Color.white;
        }


#endif
	}
}

