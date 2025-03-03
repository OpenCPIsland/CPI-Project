// (c) Copyright HutongGames, LLC 2020. All rights reserved.

using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace HutongGames.PlayMakerEditor
{
    [CustomActionEditor(typeof(HutongGames.PlayMaker.Actions.SmoothLookAt2d))]
    public class SmoothLookAt2dEditor : CustomActionEditor
    {
        private readonly JointAngularLimitHandle angularLimitHandle = new JointAngularLimitHandle();

        public override void OnEnable()
        {
            angularLimitHandle.xMotion = ConfigurableJointMotion.Locked;
            angularLimitHandle.yMotion = ConfigurableJointMotion.Locked;

            angularLimitHandle.xHandleColor = Color.clear;
            angularLimitHandle.yHandleColor = Color.clear;
            angularLimitHandle.zHandleColor = Color.white;
            
            angularLimitHandle.zRange = new Vector2(-1e+6f, 1e+6f);
        }

        public override bool OnGUI()
        {
            return DrawDefaultInspector();
        }

        public override void OnSceneGUI()
        {
            var action = target as HutongGames.PlayMaker.Actions.SmoothLookAt2d;
            if (action == null) // shouldn't happen!
            {
                return;
            }

            var go = action.Fsm.GetOwnerDefaultTarget(action.gameObject);
            if (go == null) return;

            var transform = go.transform;
            var handleSize = HandleUtility.GetHandleSize(transform.position);
            angularLimitHandle.radius = 2f * handleSize;

            var handleDirection = Quaternion.Euler(0, 0, action.rotationOffset.Value) * transform.right;

            using (new Handles.DrawingScope(PlayMakerPrefs.ArrowColor)) 
            {
                // Arrow is a little heavy. Would be nice to have smaller flat arrow style...
                //Handles.ArrowHandleCap(0, transform.position, lookRotation, minHandle.radius*1.2f, EventType.Repaint);
                Handles.DrawLine(transform.position, transform.position + handleDirection * handleSize * 2.3f);

                if (action.useLimits.Value)
                {
                    angularLimitHandle.zMin = -action.maxAngle.Value;
                    angularLimitHandle.zMax = -action.minAngle.Value;

                    var rotation = transform.parent != null ? transform.parent.rotation : Quaternion.identity;
                    var handleMatrix = Matrix4x4.TRS(go.transform.position, rotation, Vector3.one);

                    EditorGUI.BeginChangeCheck();

                    using (new Handles.DrawingScope(handleMatrix))
                    {
                        angularLimitHandle.DrawHandle();
                    }

                    if (EditorGUI.EndChangeCheck())
                    {
                        action.minAngle.Value = -angularLimitHandle.zMax;
                        action.maxAngle.Value = -angularLimitHandle.zMin;
                    }
                }
            }
        }
    }
}
