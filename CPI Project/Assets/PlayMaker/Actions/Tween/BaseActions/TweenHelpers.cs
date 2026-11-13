﻿using System;
using HutongGames.PlayMaker.TweenEnums;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HutongGames.PlayMaker.Actions
{
    public static class TweenHelpers
    {            
        /// <summary>
        /// Returns a target rotation in world space given the specified parameters
        /// Some parameters are interpreted differently based on RotationOptions selected.
        /// E.g. used by TweenRotation
        /// </summary>
        /// <param name="option">Rotation options exposed to user</param>
        /// <param name="owner">The transform being rotated</param>
        /// <param name="target">A potential target transform</param>
        /// <param name="rotation">A potential target rotation</param>
        /// <returns></returns>
        public static Quaternion GetTargetRotation(RotationOptions option, Transform owner, Transform target, Vector3 rotation)
        {
            if (owner == null) return Quaternion.identity;

            switch (option)
            {
                case RotationOptions.CurrentRotation:
                    return owner.rotation;

                case RotationOptions.WorldRotation:
                    return Quaternion.Euler(rotation);

                case RotationOptions.LocalRotation: 
                    // same as world rotation if no parent
                    if (owner.parent == null) return Quaternion.Euler(rotation);
                    return owner.parent.rotation * Quaternion.Euler(rotation);

                case RotationOptions.WorldOffsetRotation:
                    // same as rotating with global in editor
                    return Quaternion.Euler(rotation) * owner.rotation;

                case RotationOptions.LocalOffsetRotation:
                    return owner.rotation * Quaternion.Euler(rotation);

                case RotationOptions.MatchGameObjectRotation:
                    if (target == null) return owner.rotation;
                    return target.rotation * Quaternion.Euler(rotation);

                default:
                    throw new ArgumentOutOfRangeException();
            }

            //return owner.rotation; // leave as is
        }

        public static bool GetTargetRotation(RotationOptions option, Transform owner, FsmVector3 rotation,
            FsmGameObject target, out Quaternion targetRotation)
        {
            targetRotation = Quaternion.identity;
            if (owner == null || !CanEditTargetRotation(option, rotation, target)) return false;
            targetRotation = GetTargetRotation(option, owner, 
                 target.Value != null ?  target.Value.transform : null, 
                rotation.Value);
            return true;
        }

        private static bool CanEditTargetRotation(RotationOptions option, NamedVariable rotation, FsmGameObject target)
        {
            if (target == null) return false;

            switch (option)
            {
                case RotationOptions.CurrentRotation:
                    return false;
                case RotationOptions.WorldRotation:
                case RotationOptions.LocalRotation:
                case RotationOptions.WorldOffsetRotation:
                case RotationOptions.LocalOffsetRotation:
                    return !rotation.IsNone;
                    
                case RotationOptions.MatchGameObjectRotation:
                    return target.Value != null;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static Vector3 GetTargetScale(ScaleOptions option, Transform owner, Transform target, Vector3 scale)
        {
            if (owner == null) return Vector3.one;

            switch (option)
            {
                case ScaleOptions.CurrentScale:
                    return owner.localScale;

                case ScaleOptions.LocalScale:
                    return scale;

                case ScaleOptions.MultiplyScale:
                    var localScale = owner.localScale;
                    return new Vector3(localScale.x * scale.x, localScale.y * scale.y, localScale.z * scale.z);

                case ScaleOptions.AddToScale:
                    var localScale1 = owner.localScale;
                    return new Vector3(localScale1.x + scale.x, localScale1.y + scale.y, localScale1.z + scale.z);

                case ScaleOptions.MatchGameObject:
                    if (target == null) return owner.localScale;
                    return target.localScale;

                /* Useful...?
                case ScaleOptions.MatchGameObjectMultiply:
                    if (target == null) return owner.localScale;
                    if (scale == Vector3.one) return target.localScale;
                    return new Vector3(target.localScale.x * scale.x, target.localScale.y * scale.y, target.localScale.z * scale.z);
                */
            }

            return owner.localScale; // leave as is
        }

        public static bool GetTargetPosition(PositionOptions option, Transform owner, FsmVector3 position,
            FsmGameObject target, out Vector3 targetPosition)
        {
            targetPosition = Vector3.zero;
            if (owner == null || !IsValidTargetPosition(option, position, target)) return false;
            targetPosition = GetTargetPosition(option, owner, 
                target != null && target.Value != null ? target.Value.transform : null, 
                position != null ? position.Value : Vector3.zero);
            return true;
        }

        private static bool IsValidTargetPosition(PositionOptions option, NamedVariable position, FsmGameObject target)
        {
            switch (option)
            {
                case PositionOptions.CurrentPosition:
                    return true;
                case PositionOptions.WorldPosition:
                case PositionOptions.LocalPosition:
                case PositionOptions.WorldOffset:
                case PositionOptions.LocalOffset:
                    return !position.IsNone;
                    
                case PositionOptions.TargetGameObject:
                    return target.Value != null;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static bool CanEditTargetPosition(PositionOptions option, NamedVariable position, FsmGameObject target)
        {
            switch (option)
            {
                case PositionOptions.CurrentPosition:
                    return false;
                case PositionOptions.WorldPosition:
                case PositionOptions.LocalPosition:
                case PositionOptions.WorldOffset:
                case PositionOptions.LocalOffset:
                    return !position.IsNone;
                    
                case PositionOptions.TargetGameObject:
                    return target.Value != null;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static Vector3 GetTargetPosition(PositionOptions option, Transform owner, Transform target, Vector3 position)
        {
            if (owner == null) return Vector3.zero;

            switch (option)
            {
                case PositionOptions.CurrentPosition:

                    return owner.position;
                    
                case PositionOptions.WorldPosition:

                    return position;
                    
                case PositionOptions.LocalPosition:

                    if (owner.parent == null) return position;
                    return owner.parent.TransformPoint(position);
                    
                case PositionOptions.WorldOffset:

                    return owner.position + position;

                case PositionOptions.LocalOffset:

                    return owner.TransformPoint(position);
                    
                case PositionOptions.TargetGameObject:

                    if (target == null) return owner.position;
                    if (position != Vector3.zero) return target.TransformPoint(position);
                    return target.position;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static Vector3 GetUiTargetPosition(UiPositionOptions option, RectTransform owner, Transform target, Vector3 position)
        {
            if (owner == null) return Vector3.zero;

            switch (option)
            {
                case UiPositionOptions.CurrentPosition:
                    
                    return owner.anchoredPosition3D;
                
                case UiPositionOptions.Position:
                    
                    return position;
                
                case UiPositionOptions.Offset:

                    return owner.anchoredPosition3D + position;
                
                case UiPositionOptions.OffscreenTop:
                    
                    var offscreen = owner.anchoredPosition3D;
                    var rect = GetWorldRect(owner);
                    offscreen.y += Screen.height - rect.yMin;
                    return offscreen;
                
                case UiPositionOptions.OffscreenBottom:

                    offscreen = owner.anchoredPosition3D;
                    rect = GetWorldRect(owner);
                    offscreen.y -= rect.yMax;
                    return offscreen;
                
                case UiPositionOptions.OffscreenLeft:

                    offscreen = owner.anchoredPosition3D;
                    rect = GetWorldRect(owner);
                    offscreen.x -= rect.xMax;
                    return offscreen;
                
                case UiPositionOptions.OffscreenRight:

                    offscreen = owner.anchoredPosition3D;
                    rect = GetWorldRect(owner);
                    offscreen.x += Screen.width - rect.xMin;
                    return offscreen;

                case UiPositionOptions.TargetGameObject:

                    if (target == null) return owner.anchoredPosition3D;
                    if (position != Vector3.zero) return target.TransformPoint(position);
                    return target.position;

                default:
                    throw new ArgumentOutOfRangeException("option", option, null);
            }
        }

        public static Rect GetWorldRect(RectTransform rectTransform)
        {
            var v = new Vector3[4];
            rectTransform.GetWorldCorners(v);

            var maxY = Mathf.Max (v [0].y, v [1].y, v [2].y, v [3].y);
            var minY = Mathf.Min (v [0].y, v [1].y, v [2].y, v [3].y);
            var maxX = Mathf.Max (v [0].x, v [1].x, v [2].x, v [3].x);
            var minX = Mathf.Min (v [0].x, v [1].x, v [2].x, v [3].x);

            return new Rect(minX, minY, maxX-minX, maxY-minY);
        }

#if UNITY_EDITOR

        public static Vector3 DoTargetPositionHandle(Vector3 worldPos, PositionOptions option, Transform owner, FsmGameObject target)
        {
            //var worldPos = GetTargetPosition(option, owner, target, position);

            EditorGUI.BeginChangeCheck();

            var rotation = Quaternion.identity;
            var newPos = worldPos;
           
            switch (option)
            {
                case PositionOptions.CurrentPosition:
                    break;

                case PositionOptions.WorldPosition:
                    newPos = Handles.PositionHandle(worldPos, rotation);
                    break;

                case PositionOptions.LocalPosition:
                    if (owner.parent != null)
                    {
                        var parent = owner.parent;
                        rotation = parent.transform.rotation;
                        newPos = parent.InverseTransformPoint(Handles.PositionHandle(worldPos, rotation));
                    }
                    else
                    {
                        newPos = Handles.PositionHandle(worldPos, rotation);
                    }
                    break;

                case PositionOptions.WorldOffset:
                    newPos = Handles.PositionHandle(worldPos, rotation) - owner.position;
                    break;

                case PositionOptions.LocalOffset:
                    rotation = owner.rotation;
                    newPos = owner.InverseTransformPoint(Handles.PositionHandle(worldPos, rotation)) ;
                    break;

                case PositionOptions.TargetGameObject:
                    if (target.Value == null) return worldPos;
                    rotation = target.Value.transform.rotation;
                    newPos = target.Value.transform.InverseTransformPoint(Handles.PositionHandle(worldPos, rotation));
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(owner, "Move Scene Gizmo");
            }

            return newPos;
        }


#endif

    }
}