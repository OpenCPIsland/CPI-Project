// (c) Copyright HutongGames, all rights reserved.

#if (UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2)
#define UNITY_PRE_5_3
#endif

// NOTE: The new Input System and legacy Input Manager can both be enabled in a project.
// Most actions were developed for the old input manager, so we will use it if its available. 
// If only the new input system is available we will try to use that instead,
// but there might be subtle differences in the behaviour in the new system!
// We will add new helpers for the new Input System.

#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
#define NEW_INPUT_SYSTEM_ONLY
#endif


#define FSM_LOG

#if !PLAYMAKER_NO_UI
using UnityEngine.UI;
#endif

using System;
using System.Collections.Generic;
//using System.Text.RegularExpressions;
using UnityEngine;
using Random = UnityEngine.Random;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace HutongGames.PlayMaker
{

    /// <summary>
    /// Helper functions to make authoring Actions simpler.
    /// </summary>
    public static class ActionHelpers
    {
        /// <summary>
        /// Get a small white texture
        /// </summary>
        public static Texture2D WhiteTexture
        {
            // Used to make a texture, but Unity added this:
            get { return Texture2D.whiteTexture; }
        }

        /// <summary>
        /// Common blend operations for colors
        /// E.g. used by TweenColor action
        /// </summary>
        /// <returns></returns>
        public static Color BlendColor(ColorBlendMode blendMode, Color c1, Color c2)
        {
            switch (blendMode)
            {
                case ColorBlendMode.Normal:
                    return Color.Lerp(c1, c2, c2.a);

                case ColorBlendMode.Multiply:
                    return Color.Lerp(c1, c1 * c2, c2.a);

                case ColorBlendMode.Screen:
                    var screen = Color.white - (Color.white - c1) * (Color.white - c2);
                    return Color.Lerp(c1, screen, c2.a);

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Check the visibility of the Renderer on a GameObject
        /// </summary>
        public static bool IsVisible(GameObject go)
        {
            if (go == null) return false;
            var renderer = go.GetComponent<Renderer>();
            return renderer != null && renderer.isVisible;
        }

        /// <summary>
        /// Check the visibility of a GameObject's bounds to a specific camera
        /// </summary>
        public static bool IsVisible(GameObject go, Camera camera, bool useBounds)
        {
            if (go == null || camera == false) return false;
            var renderer = go.GetComponent<Renderer>();
            if (renderer == null) return false;

            if (useBounds)
                return GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(camera), renderer.bounds);

            var screenPoint = camera.WorldToViewportPoint(go.transform.position);
            return screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
        }

        /// <summary>
        /// Get the GameObject targeted by an action's FsmOwnerDefault variable
        /// </summary>
        public static GameObject GetOwnerDefault(FsmStateAction action, FsmOwnerDefault ownerDefault)
        {
            return action.Fsm.GetOwnerDefaultTarget(ownerDefault);
        }

        /// <summary>
        /// Get the first Playmaker FSM on a game object.
        /// </summary>
        public static PlayMakerFSM GetGameObjectFsm(GameObject go, string fsmName)
        {
            if (!string.IsNullOrEmpty(fsmName))
            {
                var fsmComponents = go.GetComponents<PlayMakerFSM>();

                foreach (var fsmComponent in fsmComponents)
                {
                    if (fsmComponent.FsmName == fsmName)
                    {
                        return fsmComponent;
                    }
                }

                Debug.LogWarning("Could not find FSM: " + fsmName + " on GameObject: " + go.name);
            }

            return go.GetComponent<PlayMakerFSM>();
        }

        /// <summary>
        /// Given an array of weights, returns a randomly selected index. 
        /// </summary>
        public static int GetRandomWeightedIndex(FsmFloat[] weights)
        {
            float totalWeight = 0;

            foreach (var t in weights)
            {
                totalWeight += t.Value;
            }

            var random = Random.Range(0, totalWeight);

            for (var i = 0; i < weights.Length; i++)
            {
                if (random < weights[i].Value)
                {
                    return i;
                }

                random -= weights[i].Value;
            }

            return -1;
        }

        /// <summary>
        /// Add an animation clip to a GameObject if it has an Animation component
        /// </summary>
        public static void AddAnimationClip(GameObject go, AnimationClip animClip)
        {
            if (animClip == null) return;
            var animationComponent = go.GetComponent<Animation>();
            if (animationComponent != null)
            {
                animationComponent.AddClip(animClip, animClip.name);
            }
        }

        /// <summary>
        /// Check if an animation has finished playing.
        /// </summary>
        public static bool HasAnimationFinished(AnimationState anim, float prevTime, float currentTime)
        {
            // looping animations never finish
            if (anim.wrapMode == WrapMode.Loop || anim.wrapMode == WrapMode.PingPong)
            {
                return false;
            }

            // Default and Once reset to zero when done
            if (anim.wrapMode == WrapMode.Default || anim.wrapMode == WrapMode.Once)
            {
                if (prevTime > 0 && currentTime.Equals(0))
                {
                    return true;
                }
            }

            // Time keeps going up in other modes
            return prevTime < anim.length && currentTime >= anim.length;
        }


        // Given an FsmGameObject parameter and an FsmVector3 parameter, returns a world position.
        // Many actions let you define a GameObject and/or a Position...
        public static Vector3 GetPosition(FsmGameObject fsmGameObject, FsmVector3 fsmVector3)
        {
            Vector3 finalPos;

            if (fsmGameObject.Value != null)
            {
                finalPos = !fsmVector3.IsNone ? fsmGameObject.Value.transform.TransformPoint(fsmVector3.Value) : fsmGameObject.Value.transform.position;
            }
            else
            {
                finalPos = fsmVector3.Value;
            }

            return finalPos;
        }

        /*
        public static bool GetPosition(PositionOptions options, GameObject go, FsmGameObject target,
            FsmVector3 position, out Vector3 finalPos)
        {
            var validPos = false;
            finalPos = Vector3.zero;

            if (go == null || target == null || position == null)
                return false;

            switch (options)
            {
                case PositionOptions.CurrentPosition:
                    finalPos = go.transform.position;
                    validPos = true;
                    break;

                case PositionOptions.WorldPosition:
                    if (!position.IsNone)
                    {
                        finalPos = position.Value;
                        validPos = true;
                    }
                break;

                case PositionOptions.GameObject:
                    if (target.Value != null)
                    {
                        finalPos = target.Value.transform.position;
                        validPos = true;
                    }
                    break;

                case PositionOptions.GameObjectWithOffset:
                    if (target != null)
                    {
                        finalPos = GetPosition(target, position);
                        validPos = true;
                    }
                    break;



                case PositionOptions.WorldOffset:
                    finalPos = go.transform.position + position.Value;
                    validPos = true;
                    break;

                case PositionOptions.LocalOffset:
                    finalPos = go.transform.position + go.transform.InverseTransformPoint(position.Value);
                    validPos = true;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            return validPos;
        }*/

        #region Input Helpers
        
        // Input System agnostic methods

        public static Vector3 GetDeviceAcceleration()
        {
#if NEW_INPUT_SYSTEM_ONLY
            return Accelerometer.current != null ? Accelerometer.current.acceleration.ReadValue() : Vector3.zero;
#else
            return Input.acceleration;
#endif 
        }

        public static Vector3 GetMousePosition()
        {
#if NEW_INPUT_SYSTEM_ONLY
            if (Mouse.current == null) return Vector3.zero;
            return Mouse.current.position.ReadValue();
#else
            return Input.mousePosition;
#endif
        }

        public static bool AnyKeyDown()
        {
#if NEW_INPUT_SYSTEM_ONLY
            return Keyboard.current.anyKey.isPressed ||
                   Mouse.current.leftButton.isPressed ||
                   Mouse.current.rightButton.isPressed ||
                   Mouse.current.middleButton.isPressed;
#else
            return Input.anyKeyDown;
#endif
        }

        #endregion

        // Raycast helpers that cache values to minimize the number of raycasts

        #region MousePick

        public static RaycastHit mousePickInfo;
        static float mousePickRaycastTime;
        static float mousePickDistanceUsed;
        static int mousePickLayerMaskUsed;

        public static bool IsMouseOver(GameObject gameObject, float distance, int layerMask)
        {
            if (gameObject == null) return false;
            return gameObject == MouseOver(distance, layerMask);
        }

        public static RaycastHit MousePick(float distance, int layerMask)
        {
            if (!mousePickRaycastTime.Equals(Time.frameCount) ||
                mousePickDistanceUsed < distance ||
                mousePickLayerMaskUsed != layerMask)
            {
                DoMousePick(distance, layerMask);
            }

            // otherwise use cached info

            return mousePickInfo;
        }

        public static GameObject MouseOver(float distance, int layerMask)
        {
            if (!mousePickRaycastTime.Equals(Time.frameCount) ||
                mousePickDistanceUsed < distance ||
                mousePickLayerMaskUsed != layerMask)
            {
                DoMousePick(distance, layerMask);
            }

            if (mousePickInfo.collider != null)
            {
                if (mousePickInfo.distance < distance)
                {
                    return mousePickInfo.collider.gameObject;
                }
            }

            return null;
        }

        static void DoMousePick(float distance, int layerMask)
        {
            if (Camera.main == null)
            {
                return;
            }
#if NEW_INPUT_SYSTEM_ONLY
            if (Mouse.current == null) return;
            var ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
#else
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
#endif

            Physics.Raycast(ray, out mousePickInfo, distance, layerMask);

            mousePickLayerMaskUsed = layerMask;
            mousePickDistanceUsed = distance;
            mousePickRaycastTime = Time.frameCount;
        }

#endregion

        public static int LayerArrayToLayerMask(FsmInt[] layers, bool invert)
        {
            var layermask = 0;

            foreach (var layer in layers)
            {
                layermask |= 1 << layer.Value;
            }

            if (invert)
            {
                layermask = ~layermask;
            }

            // Unity 5.3 changed this Physics property name
            //public const int kDefaultRaycastLayers = -5;
            /*
#if UNITY_PRE_5_3
                        return layermask == 0 ? Physics.kDefaultRaycastLayers : layermask;
#else
                        return layermask == 0 ? Physics.DefaultRaycastLayers : layermask;
#endif
            */
            // HACK just return the hardcoded value to avoid separate Unity 5.3 dll
            // TODO Revisit in future version
            return layermask == 0 ? -5 : layermask;
        }

        // Does a wrap mode loop? (no finished event)
        public static bool IsLoopingWrapMode(WrapMode wrapMode)
        {
            return wrapMode == WrapMode.Loop || wrapMode == WrapMode.PingPong;
        }

        public static string CheckRayDistance(float rayDistance)
        {
            return rayDistance <= 0 ? "Ray Distance should be greater than zero!\n" : "";
        }

        /// <summary>
        /// Check if a state responds to an event.
        /// Not really needed since the ErrorChecker covers this.
        /// </summary>
        public static string CheckForValidEvent(FsmState state, string eventName)
        {
            if (state == null)
            {
                return "Invalid State!";
            }

            if (string.IsNullOrEmpty(eventName))
            {
                return "";
            }

            foreach (var transition in state.Fsm.GlobalTransitions)
            {
                if (transition.EventName == eventName)
                {
                    return "";
                }
            }

            foreach (var transition in state.Transitions)
            {
                if (transition.EventName == eventName)
                {
                    return "";
                }
            }

            return "Fsm will not respond to Event: " + eventName;
        }

#region Physics setup helpers

        //[Obsolete("Use CheckPhysicsSetup(gameObject) instead")]
        public static string CheckPhysicsSetup(FsmOwnerDefault ownerDefault)
        {
            if (ownerDefault == null) return "";

            return CheckPhysicsSetup(ownerDefault.GameObject.Value);
        }

        //[Obsolete("Use CheckPhysicsSetup(gameObject) instead")]
        public static string CheckOwnerPhysicsSetup(GameObject gameObject)
        {
            return CheckPhysicsSetup(gameObject);
        }

        public static string CheckPhysicsSetup(GameObject gameObject)
        {
            var error = string.Empty;

            if (gameObject != null)
            {
                if (gameObject.GetComponent<Collider>() == null && gameObject.GetComponent<Rigidbody>() == null)
                {
                    error += "GameObject requires RigidBody/Collider!\n";
                }
            }

            return error;
        }

        //[Obsolete("Use CheckPhysics2dSetup(gameObject) instead")]
        public static string CheckPhysics2dSetup(FsmOwnerDefault ownerDefault)
        {
            if (ownerDefault == null) return "";

            return CheckPhysics2dSetup(ownerDefault.GameObject.Value);
        }

        //[Obsolete("Use CheckPhysics2dSetup(gameObject) instead")]
        public static string CheckOwnerPhysics2dSetup(GameObject gameObject)
        {
            return CheckPhysics2dSetup(gameObject);
        }

        public static string CheckPhysics2dSetup(GameObject gameObject)
        {
            var error = string.Empty;

            if (gameObject != null)
            {
                if (gameObject.GetComponent<Collider2D>() == null && gameObject.GetComponent<Rigidbody2D>() == null)
                {
                    error += "GameObject requires a RigidBody2D or Collider2D component!\n";
                }
            }

            return error;
        }

#endregion

#region Logging helpers

        public static void DebugLog(Fsm fsm, LogLevel logLevel, string text, bool sendToUnityLog = false)
        {
#if FSM_LOG
            // Logging is disabled in builds so we need to handle this
            // case separately so actions log properly in builds

            if (!Application.isEditor && sendToUnityLog)
            {
                var logText = FormatUnityLogString(text);

                switch (logLevel)
                {
                    case LogLevel.Warning:
                        Debug.LogWarning(logText);
                        break;
                    case LogLevel.Error:
                        Debug.LogError(logText);
                        break;
                    default:
                        Debug.Log(logText);
                        break;
                }	
            }

            // Note: FsmLog.LoggingEnabled is always false in builds!
            // Maybe replace this with Fsm property so we can turn on/off per Fsm?

            if (!FsmLog.LoggingEnabled || fsm == null)
            {
                return;
            }

            switch (logLevel)
            {
                case LogLevel.Info:
                    fsm.MyLog.LogAction(FsmLogType.Info, text, sendToUnityLog);
                    break;

                case LogLevel.Warning:
                    fsm.MyLog.LogAction(FsmLogType.Warning, text, sendToUnityLog);
                    break;

                case LogLevel.Error:
                    fsm.MyLog.LogAction(FsmLogType.Error, text, sendToUnityLog);
                    break;
            }
#endif
        }

        public static void LogError(string text)
        {
            DebugLog(FsmExecutionStack.ExecutingFsm, LogLevel.Error, text, true);
        }

        public static void LogWarning(string text)
        {
            DebugLog(FsmExecutionStack.ExecutingFsm, LogLevel.Warning, text, true);
        }

        /// <summary>
        /// Format a log string suitable for the Unity Log.
        /// The Unity Log lacks some context, so we bake it into the log string.
        /// </summary>
        /// <param name="text">Text to log.</param>
        /// <returns>String formatted for the Unity Log.</returns>
        public static string FormatUnityLogString(string text)
        {
            if (FsmExecutionStack.ExecutingFsm == null) return text;

            var logString = Fsm.GetFullFsmLabel(FsmExecutionStack.ExecutingFsm);

            if (FsmExecutionStack.ExecutingState != null)
            {
                logString += " : " + FsmExecutionStack.ExecutingStateName;
            }
            
            if (FsmExecutionStack.ExecutingAction != null)
            {
                logString += FsmExecutionStack.ExecutingAction.Name;
            }
            
            logString += " : " + text;

            return logString;
        }


#endregion

#region AutoName helpers

        public const string colon = ": ";
        
        public static string StripTags(string textWithTags)
        {
            // Too expensive?
            //return Regex.Replace(textWithTags, "<.*?>", string.Empty);

            textWithTags = textWithTags.Replace("<i>", "");
            textWithTags = textWithTags.Replace("</i>", "");
            textWithTags = textWithTags.Replace("<b>", "");
            textWithTags = textWithTags.Replace("</b>", "");
            return textWithTags;
        }

        public static string GetValueLabel(INamedVariable variable)
        {
#if UNITY_EDITOR
            if (variable == null) return "[null]";
            if (variable.IsNone) return "[none]";
            if (variable.UseVariable) return variable.Name;
            var rawValue = variable.RawValue;
            if (rawValue == null) return "null";
            if (rawValue is string) return "\"" + rawValue + "\"";
            if (rawValue is Array) return "Array";
            
            // A class might throw an error in ToString() so try/catch
            // see: https://hutonggames.com/playmakerforum/index.php?topic=24485
            try
            {
                if (rawValue.GetType().IsValueType) return rawValue.ToString();
                var label = rawValue.ToString();
                var classIndex = label.IndexOf('(');
                if (classIndex > 0)
                    return label.Substring(0, label.IndexOf('('));
                return label;
            }
            catch
            {
                return "";
            }

#else
            return "";
#endif
        }

        public static string GetValueLabel(Fsm fsm, FsmOwnerDefault ownerDefault)
        {
            if (ownerDefault == null) return "[null]";
            if (ownerDefault.OwnerOption == OwnerDefaultOption.UseOwner) return "Owner";
            return GetValueLabel(ownerDefault.GameObject);
        }



        /// <summary>
        /// ActionName: field1 field2 ...
        /// </summary>
        public static string AutoName(FsmStateAction action, params INamedVariable[] exposedFields)
        {
            return action == null ? null : AutoName(action.GetType().Name, exposedFields);
        }


        /// <summary>
        /// ActionName: ownerDefault
        /// </summary>
        public static string AutoName(FsmStateAction action, Fsm fsm, FsmOwnerDefault ownerDefault)
        {
            return action == null ? null : AutoName(action.GetType().Name, GetValueLabel(fsm, ownerDefault));
        }


        /// <summary>
        /// ActionName: label1 label2 ...
        /// </summary>
        public static string AutoName(FsmStateAction action, params string[] labels)
        {
            return action == null ? null : AutoName(action.GetType().Name, labels);
        }

        /// <summary>
        /// ActionName: event
        /// </summary>
        public static string AutoName(FsmStateAction action, FsmEvent fsmEvent)
        {
            return action == null ? null : AutoName(action.GetType().Name, fsmEvent != null ? fsmEvent.Name : "None");
        }

        /// <summary>
        /// ActionName: field1 field2 ...
        /// </summary>
        public static string AutoName(string actionName, params INamedVariable[] exposedFields)
        {
            var autoName = actionName + colon;
            foreach (var field in exposedFields)
            {
                autoName += GetValueLabel(field) + " ";
            }

            return autoName;
        }

        /// <summary>
        /// ActionName: field1 field2 ...
        /// </summary>
        public static string AutoName(string actionName, params string[] labels)
        {
            var autoName = actionName + colon;
            foreach (var label in labels)
            {
                autoName += label + " ";
            }

            return autoName;
        }

        /// <summary>
        /// ActionName: ownerDefault field1 field2 ...
        /// </summary>
        public static string AutoName(FsmStateAction action, Fsm fsm, FsmOwnerDefault target, params INamedVariable[] exposedFields)
        {
            return action == null ? null : AutoName(action.GetType().Name, fsm, target, exposedFields);
        }

        /// <summary>
        /// ActionName: ownerDefault field1 field2 ...
        /// </summary>
        public static string AutoName(string actionName, Fsm fsm, FsmOwnerDefault target, params INamedVariable[] exposedFields)
        {
            var autoName = actionName + colon + GetValueLabel(fsm, target) + " ";
            foreach (var field in exposedFields)
            {
                autoName += GetValueLabel(field) + " ";
            }

            return autoName;
        }

        /// <summary>
        /// ActionName: min - max
        /// </summary>
        public static string AutoNameRange(FsmStateAction action, NamedVariable min, NamedVariable max)
        {
            return action == null ? null : AutoNameRange(action.GetType().Name, min, max);
        }

        /// <summary>
        /// ActionName: min - max
        /// </summary>
        public static string AutoNameRange(string actionName, NamedVariable min, NamedVariable max)
        {
            return actionName + colon + GetValueLabel(min) + " - " + GetValueLabel(max);
        }

        /// <summary>
        /// ActionName: var = value
        /// </summary>
        public static string AutoNameSetVar(FsmStateAction action, NamedVariable var, NamedVariable value)
        {
            return action == null ? null : AutoNameSetVar(action.GetType().Name, var, value);
        }

        /// <summary>
        /// ActionName: var = value
        /// </summary>
        public static string AutoNameSetVar(string actionName, NamedVariable var, NamedVariable value)
        {
            return actionName + colon + GetValueLabel(var) + " = " + GetValueLabel(value);
        }

        /// <summary>
        /// [-Convert]ActionName: fromVar to toVar
        /// </summary>
        public static string AutoNameConvert(FsmStateAction action, NamedVariable fromVariable, NamedVariable toVariable)
        {
            return action == null ? null : AutoNameConvert(action.GetType().Name, fromVariable, toVariable);
        }

        /// <summary>
        /// [-Convert]ActionName: fromVar to toVar
        /// </summary>
        public static string AutoNameConvert(string actionName, NamedVariable fromVariable, NamedVariable toVariable)
        {
            return actionName.Replace("Convert","") + colon + GetValueLabel(fromVariable) + " to " + GetValueLabel(toVariable);
        }

        /// <summary>
        /// ActionName: property -> store
        /// </summary>
        public static string AutoNameGetProperty(FsmStateAction action, NamedVariable property, NamedVariable store)
        {
            return action == null ? null : AutoNameGetProperty(action.GetType().Name, property, store);
        }

        /// <summary>
        /// ActionName: property -> store
        /// </summary>
        public static string AutoNameGetProperty(string actionName, NamedVariable property, NamedVariable store)
        {
            return actionName + colon + GetValueLabel(property) + " -> " + GetValueLabel(store);
        }

#endregion

#region Editor helpers

#if UNITY_EDITOR

        /// <summary>
        /// Gets a rect that fits in the controls column of an inspector.
        /// </summary>
        /// <param name="height">Desired height.</param>
        public static Rect GetControlPreviewRect(float height)
        {
            var rect = GUILayoutUtility.GetRect(100f, 3000f, height, height);
            var labelWidth = EditorGUIUtility.labelWidth;
            rect.x += labelWidth + 5;
            rect.width -= labelWidth + 30;
            return rect;
        }

        private static Vector2 axisCenter, xAxisMin, xAxisMax, yAxisMin, yAxisMax;

        public static Vector2 DrawAxisXY(Rect area)
        {
            var handlesColor = Handles.color;

            axisCenter.x = area.x + area.width * 0.5f;
            axisCenter.y = area.y + area.height * 0.5f;

            xAxisMin = axisCenter;
            xAxisMax.x = xAxisMin.x + 10;
            xAxisMax.y = xAxisMin.y;

            yAxisMin = axisCenter;
            yAxisMax.x = yAxisMin.x;
            yAxisMax.y = yAxisMin.y - 10;

            //var xAxisColor = ;
            //xAxisColor.a = 0.5f;
            Handles.color = Handles.xAxisColor;
            Handles.DrawLine(xAxisMin, xAxisMax);
            
            //var yAxisColor = Color.green;
            //yAxisColor.a = 0.5f;
            Handles.color = Handles.yAxisColor;
            Handles.DrawLine(yAxisMin, yAxisMax);

            Handles.color = handlesColor;

            return axisCenter;
        }

        public static void DrawCircle(Vector2 center, float radius, Color color, float alpha)
        {
            color.a = alpha;
            DrawCircle(center, radius, color);
        }

        public static void DrawCircle(Vector2 center, float radius, Color color)
        {
            var handlesColor = Handles.color;
            Handles.color = color;
            DrawCircle(center, radius);
            Handles.color = handlesColor;
        }

        public static void DrawCircle(Vector2 center, float radius)
        {
            Handles.DrawWireArc(center, Vector3.forward, Vector3.left, 360, radius);
        }

        public static void DrawOval(Vector2 center, float xRadius, float yRadius)
        {
            var matrix = Handles.matrix;

            var yScale = yRadius / xRadius;
            if (yScale < 0.00001f) yScale = 0.00001f;
            Handles.matrix *= Matrix4x4.Scale(new Vector3(1, yScale,1f));
            center.y /= yScale;
            Handles.DrawWireArc(center, Vector3.forward, Vector3.left, 360, xRadius);

            Handles.matrix = matrix;
        }

        public static void DrawSpoke(Vector2 center, float angle, float minLength, float maxLength, float xScale = 1f, float yScale = 1f)
        {
            Vector2 start, end;

            var radians = Mathf.Deg2Rad * angle;
            start.x = axisCenter.x + Mathf.Cos(radians) * minLength * xScale;
            start.y = axisCenter.y + Mathf.Sin(radians) * minLength * yScale;
            end.x = axisCenter.x + Mathf.Cos(radians) * maxLength * xScale;
            end.y = axisCenter.y + Mathf.Sin(radians) * maxLength * yScale;

            Handles.DrawLine(start, end);
        }

        public static void DrawArc(Vector2 center, float fromAngle, float toAngle, float xRadius, float yRadius)
        {
            var matrix = Handles.matrix;

            var yScale = yRadius / xRadius;
            if (yScale < 0.00001f) yScale = 0.00001f;
            Handles.matrix *= Matrix4x4.Scale(new Vector3(1, yScale,1f));
            center.y /= yScale;

            var radians = Mathf.Deg2Rad * fromAngle;
            var from = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));
            Handles.DrawWireArc(center, Vector3.forward, from, toAngle - fromAngle, xRadius);

            Handles.matrix = matrix;
        }

        private static Rect tempRect = new Rect();
        public static void DrawRect(Vector2 center, float xLength, float yLength)
        {
            tempRect.Set(center.x-xLength, center.y-yLength, xLength*2, yLength*2);
            DrawRect(tempRect);
        }

        public static void DrawRect(Rect rect, Color color, float alpha)
        {
            color.a = alpha;
            DrawRect(rect, color);
        }

        public static void DrawRect(Rect rect, Color color)
        {
            var handlesColor = Handles.color;
            Handles.color = color;
            DrawRect(rect);
            Handles.color = handlesColor;
        }

        public static void DrawRect(Rect rect)
        {
            var framePoints = new Vector3[5];
            framePoints[0] = new Vector3(rect.x, rect.y);
            framePoints[1] = new Vector3(rect.xMax, rect.y);
            framePoints[2] = new Vector3(rect.xMax, rect.yMax);
            framePoints[3] = new Vector3(rect.x, rect.yMax);
            framePoints[4] = new Vector3(rect.x, rect.y);

            Handles.DrawPolyLine(framePoints);
        }

        /// <summary>
        /// Draws a Position Handle in the scene using a combination of GameObject and Position values.
        /// If a GameObject is specified, the Position is a local offset.
        /// If no GameObject is specified, the Position is a world position.
        /// Many actions use this setup. 
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="go"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public static Vector3 PositionHandle(UnityEngine.Object owner, GameObject go, Vector3 position)
        {
            EditorGUI.BeginChangeCheck();

            Transform transform = null;
            var rotation = Quaternion.identity;            
            var worldPos = GetPosition(go, position);

            if (go != null)
            {
                transform = go.transform;
                rotation = transform.rotation;
                var pos = transform.position;
                Handles.Label(pos, go.name, "Box");
                Handles.DrawDottedLine(pos, worldPos, 2f);
            }

            worldPos = Handles.PositionHandle(worldPos, rotation);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(owner, "Move Scene Gizmo");
            }

            return transform != null ? transform.InverseTransformPoint(worldPos) : worldPos;
        }

        /// <summary>
        /// Draws an arrow in the scene.
        /// Useful for actions that set a direction.
        /// </summary>
        public static void DrawArrow(Vector3 fromPos, Vector3 toPos, Color color, float arrowScale = 0.2f)
        {
            var direction = toPos - fromPos;
            if (direction.sqrMagnitude < 0.0001f) return;

            var lookAtRotation = Quaternion.LookRotation(direction);
            var distance = Vector3.Distance(fromPos, toPos);
            var handleSize = HandleUtility.GetHandleSize(toPos);
            var arrowSize = handleSize * arrowScale;

            var originalColor = Handles.color;
            Handles.color = color;

            Handles.DrawLine(fromPos, toPos);

#if UNITY_5_5_OR_NEWER
            Handles.ConeHandleCap(0, fromPos + direction.normalized * (distance - arrowSize), lookAtRotation, arrowSize, EventType.Repaint); // fudge factor to position cap correctly
#else
            Handles.ConeCap(0, fromPos + direction.normalized * (distance - arrowSize), lookAtRotation, arrowSize); // fudge factor to position cap correctly
#endif

            Handles.color = originalColor;
        }

        public static void DrawTexture(Vector2 position, Texture texture, float angle, Vector2 anchor)
        {
            var matrix = Handles.matrix;

            //Handles.matrix *= Matrix4X4Rotate(Quaternion.AngleAxis(angle, Vector3.forward));
            
            RotateHandlesAroundPivot(angle, position);
            Handles.Label(position, texture);


            Handles.matrix = matrix;
        }

        public static bool IsHotControl(bool hotControlFlag)
        {
            var tempString = GUI.GetNameOfFocusedControl();
            if (!string.IsNullOrEmpty(tempString))
            {
                Debug.Log(tempString);
            }
            
            if ((tempString == "xAxis" || tempString == "yAxis" || tempString == "zAxis") && GUIUtility.hotControl != 0)
            {
                hotControlFlag = true;  // declared elsewhere
                //Debug.Log ("dragging");
            }
             
            if ((tempString == "xAxis" || tempString == "yAxis" || tempString == "zAxis") && hotControlFlag && GUIUtility.hotControl == 0)
            {
                hotControlFlag = false;
                Debug.Log ("released");
            }

            return hotControlFlag;
        }

        public static void RotateHandlesAroundPivot(float angle, Vector3 pivotPoint)
        {
            var matrix = Handles.matrix;
            Handles.matrix = Matrix4x4.identity;
            Vector2 vector3 = pivotPoint; // GUIClip.Unclip(pivotPoint);
            Handles.matrix = Matrix4x4.TRS((Vector3) vector3, Quaternion.Euler(0.0f, 0.0f, angle), Vector3.one) * Matrix4x4.TRS((Vector3) (-vector3), Quaternion.identity, Vector3.one) * matrix;
        }

        /// <summary>
        /// Get a mesh that can be used by Gizmos.DrawMesh to preview the mesh while editing.
        /// E.g. to preview a GameObject moving to a target
        /// </summary>
        public static Mesh GetPreviewMesh(GameObject go)
        {
            if (go == null) return null;

            var meshFilters = go.GetComponentsInChildren<MeshFilter>(false);
            if (meshFilters.Length == 0) return null;

            var combineList = new List<CombineInstance>();
            foreach (var meshFilter in meshFilters)
            {
                var combine = new CombineInstance
                {
                    mesh = meshFilter.sharedMesh,
                    transform = meshFilter.transform.localToWorldMatrix
                };

                combineList.Add(combine);
            }

            var combinedMesh = new Mesh();
            combinedMesh.CombineMeshes(combineList.ToArray());

            return combinedMesh;
        }

        /// <summary>
        /// Single color version of Handles.ScaleHandle.
        /// Useful when you have multiple editors (e.g. TweenScale)
        /// Note, does not handle value of 0 very well (fix?)
        /// </summary>
        public static Vector3 SingleColorScaleHandle(GameObject go, Vector3 scale, float handleSize, Color color)
        {
            var matrix = Handles.matrix;
            Handles.matrix = go.transform.localToWorldMatrix;
            Handles.matrix *= Matrix4x4.Inverse(Matrix4x4.Scale(go.transform.localScale));

            var tempColor = Handles.color;
            Handles.color = color;

            var scaleX = Handles.ScaleSlider(scale.x, 
                Vector3.zero, -Vector3.left, Quaternion.identity, handleSize, 0f);
            var scaleY = Handles.ScaleSlider(scale.y, 
                Vector3.zero, -Vector3.down, Quaternion.identity, handleSize, 0f);
            var scaleZ = Handles.ScaleSlider(scale.z, 
                Vector3.zero, -Vector3.back, Quaternion.identity, handleSize, 0f);

            Handles.color = tempColor;
            Handles.matrix = matrix;

            scale.Set(scaleX, scaleY, scaleZ);
            return scale;
        }

        /// <summary>
        /// Get a local bounding box for a GameObject
        /// </summary>
        public static Bounds GetLocalBounds(GameObject gameObject)
        {
            // See  GetLocalBounds in InternalEditorUtilityBindings.gen.cs in unity c# ref projects

            var rectTransform = gameObject.GetComponent<RectTransform>();
            if (rectTransform)
            {
                var rect = rectTransform.rect;
                return new Bounds(rect.center, rect.size);
            }

            var renderer = gameObject.GetComponent<Renderer>();
            if (renderer is MeshRenderer)
            {
                var filter = renderer.GetComponent<MeshFilter>();
                if (filter != null && filter.sharedMesh != null)
                    return filter.sharedMesh.bounds;
            }

            var spriteRenderer = renderer as SpriteRenderer;
            if (spriteRenderer != null)
            {
                return spriteRenderer.bounds;
            }

            return new Bounds(Vector3.zero, Vector3.zero);
        }

        /// <summary>
        /// Draw wire bounding box for Transform.
        /// Optionally scale bounding box.
        /// </summary>
        public static void DrawWireBounds(Transform transform, Vector3 scale, Color color)
        {
            var matrix = Handles.matrix;
            Handles.matrix = transform.localToWorldMatrix;

            var bounds = GetLocalBounds(transform.gameObject);
            var size = bounds.size;
            size.Set(size.x * scale.x, size.y * scale.y, size.z * scale.z);

            DrawWireCube(Vector3.zero, size, color);

            Handles.matrix = matrix;
        }

        /// <summary>
        /// Draw wireframe bounding box around object with optional rotation (for editing gizmos)
        /// </summary>
        public static void DrawWireBounds(Transform transform, Quaternion rotate, Color color)
        {
            var matrix = Handles.matrix;
            Handles.matrix = Matrix4x4.TRS(transform.position, rotate, transform.lossyScale);

            DrawWireCube(Vector3.zero, GetLocalBounds(transform.gameObject).size, color);

            Handles.matrix = matrix;
        }

        public static void DrawWireBounds(Transform transform, Vector3 position, Quaternion rotation, Color color)
        {
            var matrix = Handles.matrix;
            Handles.matrix = Matrix4x4.TRS(position, rotation, transform.lossyScale);

            DrawWireCube(Vector3.zero, GetLocalBounds(transform.gameObject).size, color);

            Handles.matrix = matrix;
        }

        // Creates a rotation matrix. Note: Assumes unit quaternion
        public static Matrix4x4 Matrix4X4Rotate(Quaternion q)
        {
            // Pre-calculate coordinate products
            var x = q.x * 2.0F;
            var y = q.y * 2.0F;
            var z = q.z * 2.0F;
            var xx = q.x * x;
            var yy = q.y * y;
            var zz = q.z * z;
            var xy = q.x * y;
            var xz = q.x * z;
            var yz = q.y * z;
            var wx = q.w * x;
            var wy = q.w * y;
            var wz = q.w * z;

            // Calculate 3x3 matrix from orthonormal basis
            Matrix4x4 m;
            m.m00 = 1.0f - (yy + zz); m.m10 = xy + wz; m.m20 = xz - wy; m.m30 = 0.0F;
            m.m01 = xy - wz; m.m11 = 1.0f - (xx + zz); m.m21 = yz + wx; m.m31 = 0.0F;
            m.m02 = xz + wy; m.m12 = yz - wx; m.m22 = 1.0f - (xx + yy); m.m32 = 0.0F;
            m.m03 = 0.0F; m.m13 = 0.0F; m.m23 = 0.0F; m.m33 = 1.0F;
            return m;
        }

        /// <summary>
        /// Draw wireframe bounding box around object with optional translate, rotate, and scale (for editing gizmos)
        /// </summary>
        public static void DrawWireBounds(Transform transform, Vector3 translate, Quaternion rotate, Vector3 scale, Color color)
        {
            var matrix = Handles.matrix;
            Handles.matrix = transform.localToWorldMatrix;
            Handles.matrix *= Matrix4x4.TRS(translate, rotate, scale);
    

            DrawWireCube(Vector3.zero, GetLocalBounds(transform.gameObject).size, color);

            Handles.matrix = matrix;
        }

        /// <summary>
        /// Similar to Gizmos.DrawWireCube but can be used in editor code.
        /// </summary>
        public static void DrawWireCube(Vector3 position, Vector3 size, Color color)
        {
            var originalColor = Handles.color;
            Handles.color = color;

            var half = size / 2;
            // draw front
            Handles.DrawLine(position + new Vector3(-half.x, -half.y, half.z), position + new Vector3(half.x, -half.y, half.z));
            Handles.DrawLine(position + new Vector3(-half.x, -half.y, half.z), position + new Vector3(-half.x, half.y, half.z));
            Handles.DrawLine(position + new Vector3(half.x, half.y, half.z), position + new Vector3(half.x, -half.y, half.z));
            Handles.DrawLine(position + new Vector3(half.x, half.y, half.z), position + new Vector3(-half.x, half.y, half.z));
            // draw back
            Handles.DrawLine(position + new Vector3(-half.x, -half.y, -half.z), position + new Vector3(half.x, -half.y, -half.z));
            Handles.DrawLine(position + new Vector3(-half.x, -half.y, -half.z), position + new Vector3(-half.x, half.y, -half.z));
            Handles.DrawLine(position + new Vector3(half.x, half.y, -half.z), position + new Vector3(half.x, -half.y, -half.z));
            Handles.DrawLine(position + new Vector3(half.x, half.y, -half.z), position + new Vector3(-half.x, half.y, -half.z));
            // draw corners
            Handles.DrawLine(position + new Vector3(-half.x, -half.y, -half.z), position + new Vector3(-half.x, -half.y, half.z));
            Handles.DrawLine(position + new Vector3(half.x, -half.y, -half.z), position + new Vector3(half.x, -half.y, half.z));
            Handles.DrawLine(position + new Vector3(-half.x, half.y, -half.z), position + new Vector3(-half.x, half.y, half.z));
            Handles.DrawLine(position + new Vector3(half.x, half.y, -half.z), position + new Vector3(half.x, half.y, half.z));

            Handles.color = originalColor;
        }

        // https://forum.unity.com/threads/drawing-capsule-gizmo.354634/
        public static void DrawWireCapsule(Vector3 _pos, Vector3 _pos2, float _radius, Color _color)
        {
            var color = Handles.color;
            Handles.color = _color;

            var forward = _pos2 - _pos;
            var _rot = Quaternion.LookRotation(forward);
            var pointOffset = _radius / 2f;
            var length = forward.magnitude;
            var center2 = new Vector3(0f, 0, length);

            Matrix4x4 angleMatrix = Matrix4x4.TRS(_pos, _rot, Handles.matrix.lossyScale);

            using (new Handles.DrawingScope(angleMatrix))
            {
                Handles.DrawWireDisc(Vector3.zero, Vector3.forward, _radius);
                Handles.DrawWireArc(Vector3.zero, Vector3.up, Vector3.left * pointOffset, -180f, _radius);
                Handles.DrawWireArc(Vector3.zero, Vector3.left, Vector3.down * pointOffset, -180f, _radius);
                Handles.DrawWireDisc(center2, Vector3.forward, _radius);
                Handles.DrawWireArc(center2, Vector3.up, Vector3.right * pointOffset, -180f, _radius);
                Handles.DrawWireArc(center2, Vector3.left, Vector3.up * pointOffset, -180f, _radius);

                DrawLine(_radius, 0f, length);
                DrawLine(-_radius, 0f, length);
                DrawLine(0f, _radius, length);
                DrawLine(0f, -_radius, length);
            }

            Handles.color = color;
        }

        private static void DrawLine(float arg1, float arg2, float forward)
        {
            Handles.DrawLine(new Vector3(arg1, arg2, 0f), new Vector3(arg1, arg2, forward));
        }

#endif //UNITY_EDITOR

#endregion

#region Obsolete

        /// <summary>
        /// Actions should use this for consistent error messages.
        /// Error will contain action name and full FSM path.
        /// </summary>
        [Obsolete("Use LogError instead.")]
        public static void RuntimeError(FsmStateAction action, string error)
        {
            action.LogError(action + colon + error);
        }

#endregion

    }
}
