using HutongGames.PlayMaker.Actions;
using UnityEditor;
using UnityEngine;

namespace HutongGames.PlayMakerEditor
{
    [CustomActionEditor(typeof(HutongGames.PlayMaker.Actions.RaycastAll))]
    public class RaycastAllEditor : CustomActionEditor
    {
        private RaycastAll action;

        public override void OnEnable()
        {
            action = target as RaycastAll;
        }

        public override bool OnGUI()
        {
            return DrawDefaultInspector();
        }

        public override void OnSceneGUI()
        {
            action = target as RaycastAll;
            if (action == null) // shouldn't happen!
            {
                return;
            }

            var go = action.Fsm.GetOwnerDefaultTarget(action.fromGameObject);
            var start = go != null ? go.transform.position : action.fromPosition.Value;

            var dirVector = action.direction.Value;
            if (go != null && action.space == Space.Self)
            {
                dirVector = go.transform.TransformDirection(action.direction.Value);
            }

            var end = start + dirVector * action.distance.Value;

            Handles.DrawLine(start, end);



            if (go == null)
            {
                // Position handle for start position

                action.fromPosition.Value = Handles.PositionHandle(action.fromPosition.Value, Quaternion.identity);
            }

            EditorGUI.BeginChangeCheck();

            end = Handles.PositionHandle(end, go == null ? Quaternion.identity : 
                action.space == Space.Self ? go.transform.rotation : Quaternion.identity );

            if (EditorGUI.EndChangeCheck())
            {
                // world space
                var ray = end - start;

                if (go != null)
                {
                    if (action.space == Space.Self)
                    {
                        ray = go.transform.InverseTransformVector(ray);
                    }
                }

                action.direction = ray.normalized;
                action.distance = ray.magnitude;
            }

        }
    }
}