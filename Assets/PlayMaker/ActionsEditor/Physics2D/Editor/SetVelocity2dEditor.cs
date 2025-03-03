using System;
using HutongGames.PlayMaker;
using UnityEditor;
using UnityEngine;

namespace HutongGames.PlayMakerEditor
{
    [CustomActionEditor(typeof (HutongGames.PlayMaker.Actions.SetVelocity2d))]
    public class SetVelocity2dEditor : CustomActionEditor
    {
        public override bool OnGUI()
        {
            return DrawDefaultInspector();
        }

        private Vector3 toPos;

        public override void OnSceneGUI()
        {
            var action = target as HutongGames.PlayMaker.Actions.SetVelocity2d;
            if (action == null) // shouldn't happen!
            {
                return;
            }

            var go = action.Fsm.GetOwnerDefaultTarget(action.gameObject);
            if (go == null) return;

            var transform = go.transform;
            var space = action.space;

            var velocity = action.vector.IsNone ? Vector2.zero : action.vector.Value;
            if (!action.x.IsNone) velocity.x = action.x.Value;
            if (!action.y.IsNone) velocity.y = action.y.Value;

            if (space == Space.Self)
            {
                velocity = transform.TransformDirection(velocity);
            }

            var constrainX = action.vector.IsNone && action.x.IsNone;
            var constrainY = action.vector.IsNone && action.y.IsNone;

            var origin = transform.position;           
            toPos.x = origin.x + velocity.x;
            toPos.y = origin.y + velocity.y;
            toPos.z = origin.z;

            ActionHelpers.DrawArrow(origin, toPos, PlayMakerPrefs.ArrowColor);

            //if (EditorApplication.isPlayingOrWillChangePlaymode) return;

            // editor

            var newPosition = Handles.DoPositionHandle(toPos, space == Space.Self ? go.transform.rotation : Quaternion.identity);
            newPosition.z = origin.z;
            newPosition -= origin;

            if (space == Space.Self)
            {
                newPosition = go.transform.InverseTransformDirection(newPosition);
            }

            if (constrainX) newPosition.x = origin.x;
            if (constrainY) newPosition.y = origin.y;

            if (Math.Abs(newPosition.x) < 0.0001f) newPosition.x = 0;
            if (Math.Abs(newPosition.y) < 0.0001f) newPosition.y = 0;

            action.vector.Value = new Vector2(newPosition.x, newPosition.y);
            action.x.Value = newPosition.x;
            action.y.Value = newPosition.y;

            //ActionHelpers.DrawTexture(newPosition, FsmEditorStyles.RightArrow, 45, Vector2.zero);

            if (GUI.changed)
            {
                FsmEditor.EditingActions();
                //Debug.Log("Save Actions");
                //FsmEditor.SaveActions();
            }
        }
    }
}
