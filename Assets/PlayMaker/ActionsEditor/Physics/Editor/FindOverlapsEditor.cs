// (c) Copyright HutongGames, LLC 2010-2020. All rights reserved.  

using UnityEditor;
using UnityEngine;
using UnityEditor.IMGUI.Controls;

namespace HutongGames.PlayMakerEditor
{
    [CustomActionEditor(typeof(HutongGames.PlayMaker.Actions.FindOverlaps))]
    public class FindOverlapsEditor : CustomActionEditor
    {
        private HutongGames.PlayMaker.Actions.FindOverlaps action;
        private BoxBoundsHandle boxBoundsHandle;
        private CapsuleBoundsHandle capsuleBoundsHandle;

        public override void OnEnable()
        {
            action = target as HutongGames.PlayMaker.Actions.FindOverlaps;
        }

        public override bool OnGUI()
        {
            return DrawDefaultInspector();
        }

        public override void OnSceneGUI()
        {
            action = target as HutongGames.PlayMaker.Actions.FindOverlaps;

            if (action == null || action.debug == null || !action.debug.Value) return;

            var color = Handles.color;
            Handles.color = action.debugColor.Value;

            EditorGUI.BeginChangeCheck();

            action.InitShapeCenter();

            var transform = action.cachedTransform;

            var newCenter = Handles.PositionHandle(action.center, transform != null ? transform.rotation : Quaternion.identity);

            Handles.matrix = Matrix4x4.TRS(action.center, transform != null ? transform.rotation : Quaternion.identity, Vector3.one);

            switch (action.shape)
            {
                case HutongGames.PlayMaker.Actions.FindOverlaps.Shape.Box:

                    if (boxBoundsHandle == null) boxBoundsHandle = new BoxBoundsHandle();

                    boxBoundsHandle.center = Vector3.zero;
                    boxBoundsHandle.size = action.box.Value;

                    EditorGUI.BeginChangeCheck();
                    boxBoundsHandle.DrawHandle();
                    if (EditorGUI.EndChangeCheck())
                    {
                        action.box.Value = boxBoundsHandle.size;
                    }

                    break;

                case HutongGames.PlayMaker.Actions.FindOverlaps.Shape.Sphere:

                    action.radius.Value = Handles.RadiusHandle(Quaternion.identity, Vector3.zero, action.radius.Value);

                    break;

                case HutongGames.PlayMaker.Actions.FindOverlaps.Shape.Capsule:

                    if (capsuleBoundsHandle == null) capsuleBoundsHandle = new CapsuleBoundsHandle();

                    capsuleBoundsHandle.center = Vector3.zero;
                    capsuleBoundsHandle.height = action.height.Value;
                    capsuleBoundsHandle.radius = action.radius.Value;
                    capsuleBoundsHandle.heightAxis = CapsuleBoundsHandle.HeightAxis.Y;

                    EditorGUI.BeginChangeCheck();
                    capsuleBoundsHandle.DrawHandle();
                    if (EditorGUI.EndChangeCheck())
                    {
                        action.radius.Value = capsuleBoundsHandle.radius;
                        action.height.Value = capsuleBoundsHandle.height;
                    }

                    break;
            }

            Handles.color = color;

            if (EditorGUI.EndChangeCheck())
            {
                action.offset.Value = transform == null ?
                    newCenter : transform.InverseTransformPoint(newCenter);

                GUI.changed = true;
            }
        }
   
    }
}
