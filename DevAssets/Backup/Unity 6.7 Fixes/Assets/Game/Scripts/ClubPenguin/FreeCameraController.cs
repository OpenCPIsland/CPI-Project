using ClubPenguin.BlobShadows;
using ClubPenguin.Core;
using ClubPenguin.LOD;
using Disney.Kelowna.Common.DataModel;
using Disney.Kelowna.Common.SEDFSM;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using Tweaker.Core;
using Tweaker.UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ClubPenguin
{
    public class FreeCameraController : MonoBehaviour
    {
        private const string LeftHorizontal = "LeftHorizontal";
        private const string LeftVertical = "LeftVertical";
        private const string RightHorizontal = "RightHorizontal";
        private const string RightVertical = "RightVertical";
        private const string LeftTrigger = "LeftTrigger";
        private const string RightTrigger = "RightTrigger";
        private const string LeftBumper = "LeftBumper";
        private const string RightBumper = "RightBumper";
        private const string StartButton = "StartButton";
        private const string BButton = "BButton";
        private const string XButton = "XButton";
        private const string YButton = "YButton";
        private const string DPadLeft = "DPadLeft";
        private const string DPadRight = "DPadRight";
        private const string DPadUp = "DPadUp";
        private const float defaultFOV = 60f;

        private const float InputDeadzone = 0.1f;

        private const string XSpeedPlayerPrefsKey = "FreeCamera.XSpeed";
        private const string YSpeedPlayerPrefsKey = "FreeCamera.YSpeed";
        private const string ZSpeedPlayerPrefsKey = "FreeCamera.ZSpeed";
        private const string XSensitivityPlayerPrefsKey = "FreeCamera.XSensitivity";
        private const string YSensitivityPlayerPrefsKey = "FreeCamera.YSensitivity";
        private const string ZSensitivityPlayerPrefsKey = "FreeCamera.ZSensitivity";
        private const string RotationModifierFOVPlayerPrefsKey = "FreeCamera.RotationModifierFOV";
        private const string XSpeedModifierFOVPlayerPrefsKey = "FreeCamera.XSpeedModifierFOV";
        private const string YSpeedModifierFOVPlayerPrefsKey = "FreeCamera.YSpeedModifierFOV";
        private const string ZSpeedModifierFOVPlayerPrefsKey = "FreeCamera.ZSpeedModifierFOV";
        private const string BumperRotationSensitivityPlayerPrefsKey = "FreeCamera.ControllerSensitivity";

        public Transform Target;
        public Camera Camera;
        public float XSensitivity = 1f;
        public float YSensitivity = 1f;
        public float ZSensitivity = 1f;
        public float BumperRotationSensitivity = 0.5f;
        public float XSpeed = 0.3f;
        public float YSpeed = 0.3f;
        public float ZSpeed = 0.3f;
        public bool UseDefaultSpeeds;
        public float KeyboardSpeedMultiplier = 0.4f; // The speed for the camera controls via the the keyboard. - Malcolm
        public bool WorldRelativeZMotion;
        public float RotationModifierFOV = 0.8f;
        public float XSpeedModifierFOV = 0.7f;
        public float YSpeedModifierFOV = 0.7f;
        public float ZSpeedModifierFOV = 0.7f;

        private bool wasLeftTriggerPressed;
        private bool wasRightTriggerPressed;
        private StateMachineContext context;
        private BlobShadowCaster blobShadowCaster;
        private GroupCulling groupCulling;
        private Canvas worldChatCanvas;
        private Camera mainCamera;
        private int localPlayerMask;
        private bool isLocalPlayerActive;

        private float ApplyDeadzone(float value)
        {
            if (Mathf.Abs(value) < InputDeadzone)
                return 0f;
            return (value - Mathf.Sign(value) * InputDeadzone) / (1f - InputDeadzone);
        }

        private float yValue
        {
            get
            {
                var kb = Keyboard.current;
                if (kb != null)
                {
                    if (kb.wKey.isPressed) return 1f * KeyboardSpeedMultiplier;
                    if (kb.sKey.isPressed) return -1f * KeyboardSpeedMultiplier;
                }

                float leftTrigger = ApplyDeadzone(GetAxisOrButton(LeftTrigger));
                float rightTrigger = ApplyDeadzone(GetAxisOrButton(RightTrigger));

                if (wasLeftTriggerPressed && wasRightTriggerPressed)
                    return rightTrigger - leftTrigger;

                float num = 0f;
                if (Math.Abs(leftTrigger) > float.Epsilon)
                {
                    num -= leftTrigger + 1f;
                    wasLeftTriggerPressed = true;
                }
                if (Math.Abs(rightTrigger) > float.Epsilon)
                {
                    num += rightTrigger + 1f;
                    wasRightTriggerPressed = true;
                }
                return num;
            }
        }

        private void Start()
        {
            if (!UseDefaultSpeeds)
            {
                XSpeed = PlayerPrefs.GetFloat(XSpeedPlayerPrefsKey, 0.3f);
                YSpeed = PlayerPrefs.GetFloat(YSpeedPlayerPrefsKey, 0.3f);
                ZSpeed = PlayerPrefs.GetFloat(ZSpeedPlayerPrefsKey, 0.3f);
                XSensitivity = PlayerPrefs.GetFloat(XSensitivityPlayerPrefsKey, 1f);
                YSensitivity = PlayerPrefs.GetFloat(YSensitivityPlayerPrefsKey, 1f);
                ZSensitivity = PlayerPrefs.GetFloat(ZSensitivityPlayerPrefsKey, 1f);
                RotationModifierFOV = PlayerPrefs.GetFloat(RotationModifierFOVPlayerPrefsKey, 0.8f);
                XSpeedModifierFOV = PlayerPrefs.GetFloat(XSpeedModifierFOVPlayerPrefsKey, 0.7f);
                YSpeedModifierFOV = PlayerPrefs.GetFloat(YSpeedModifierFOVPlayerPrefsKey, 0.7f);
                ZSpeedModifierFOV = PlayerPrefs.GetFloat(ZSpeedModifierFOVPlayerPrefsKey, 0.7f);
                BumperRotationSensitivity = PlayerPrefs.GetFloat(BumperRotationSensitivityPlayerPrefsKey, 0.5f);
            }

            Camera = gameObject.AddComponent<Camera>();
            localPlayerMask = LayerMask.NameToLayer("LocalPlayer");
            mainCamera = Camera.main;
            if (mainCamera != null)
            {
                Camera.CopyFrom(mainCamera);
                mainCamera.enabled = false;
            }
            gameObject.tag = "MainCamera";

            GameObject trayRootObj = GameObject.FindGameObjectWithTag(UIConstants.Tags.UI_Tray_Root);
            if (trayRootObj != null)
            {
                context = trayRootObj.GetComponent<StateMachineContext>();
                context.SendEvent(new ExternalEvent("Root", "noUI"));
            }
            Service.Get<EventDispatcher>().DispatchEvent(default(PlayerNameEvents.HidePlayerNames));
            DataEntityHandle localPlayerHandle = Service.Get<CPDataEntityCollection>().LocalPlayerHandle;
            GameObjectReferenceData component;
            if (!localPlayerHandle.IsNull && Service.Get<CPDataEntityCollection>().TryGetComponent(localPlayerHandle, out component))
            {
                blobShadowCaster = component.GameObject.GetComponent<BlobShadowCaster>();
            }
            groupCulling = UnityEngine.Object.FindAnyObjectByType<GroupCulling>();
            if (groupCulling != null)
                groupCulling.enabled = false;

            GameObject worldChatObj = GameObject.Find("WorldChatCanvas");
            if (worldChatObj != null)
            {
                worldChatCanvas = worldChatObj.GetComponent<Canvas>();
                if (worldChatCanvas != null)
                    worldChatCanvas.worldCamera = Camera;
            }
            setLocalPlayerActive(false);
        }

        private void Update()
        {
            var gamepad = Gamepad.current;
            var keyboard = Keyboard.current;

            bool hasGamepad = gamepad != null;
            bool hasKeyboard = keyboard != null;

            if (!hasGamepad && !hasKeyboard)
                return;

            float horizontal = ApplyDeadzone(GetCombinedAxis(LeftHorizontal,
                hasKeyboard && keyboard.rightArrowKey.isPressed ? 1f * KeyboardSpeedMultiplier :
                hasKeyboard && keyboard.leftArrowKey.isPressed ? -1f * KeyboardSpeedMultiplier : 0f));

            float vertical = ApplyDeadzone(GetCombinedAxis(LeftVertical,
                hasKeyboard && keyboard.upArrowKey.isPressed ? 1f * KeyboardSpeedMultiplier :
                hasKeyboard && keyboard.downArrowKey.isPressed ? -1f * KeyboardSpeedMultiplier : 0f));

            Target.position += Target.right * horizontal * XSpeed * getFOVModification(XSpeedModifierFOV);

            if (WorldRelativeZMotion)
            {
                Vector3 forward = Target.forward;
                forward.y = 0f;
                Target.position += forward.normalized * vertical * ZSpeed * getFOVModification(ZSpeedModifierFOV);
            }
            else
            {
                Target.position += Target.forward * vertical * ZSpeed * getFOVModification(ZSpeedModifierFOV);
            }

            Target.position += Vector3.up * yValue * YSpeed * getFOVModification(YSpeedModifierFOV);

            float lookH = ApplyDeadzone(GetCombinedAxis(RightHorizontal,
                hasKeyboard && keyboard.dKey.isPressed ? 1f * KeyboardSpeedMultiplier :
                hasKeyboard && keyboard.aKey.isPressed ? -1f * KeyboardSpeedMultiplier : 0f));

            float lookV = ApplyDeadzone(GetAxisOrButton(RightVertical));

            Quaternion lhs = Quaternion.AngleAxis(lookH * XSensitivity * getFOVModification(RotationModifierFOV), Vector3.up);
            Target.transform.rotation = lhs * Target.transform.rotation;

            Quaternion rhs = Quaternion.AngleAxis(lookV * YSensitivity * getFOVModification(RotationModifierFOV), Vector3.left);
            Target.transform.rotation = Target.transform.rotation * rhs;

            if (GetButton(LeftBumper))
            {
                Quaternion yawLeft = Quaternion.AngleAxis(-BumperRotationSensitivity, Vector3.up);
                Target.transform.rotation = yawLeft * Target.transform.rotation;
            }
            if (GetButton(RightBumper))
            {
                Quaternion yawRight = Quaternion.AngleAxis(BumperRotationSensitivity, Vector3.up);
                Target.transform.rotation = yawRight * Target.transform.rotation;
            }

            if (GetButton(DPadLeft))
            {
                Quaternion lhs2 = Quaternion.AngleAxis(ZSensitivity, Target.forward);
                Target.transform.rotation = lhs2 * Target.transform.rotation;
            }
            if (GetButton(DPadRight))
            {
                Quaternion lhs2 = Quaternion.AngleAxis(0f - ZSensitivity, Target.forward);
                Target.transform.rotation = lhs2 * Target.transform.rotation;
            }
            if (GetButtonDown(DPadUp))
            {
                Vector3 eulerAngles = Target.transform.rotation.eulerAngles;
                eulerAngles.z = 0f;
                Target.transform.rotation = Quaternion.Euler(eulerAngles);
            }
            if (GetButtonDown(StartButton))
                Service.Get<TweakerConsoleController>().gameObject.SetActive(true);
            if (GetButtonDown(BButton))
                Service.Get<TweakerConsoleController>().gameObject.SetActive(false);
            if (GetButtonDown(XButton))
                WorldRelativeZMotion = !WorldRelativeZMotion;
            if (GetButtonDown(YButton))
                setLocalPlayerActive(!isLocalPlayerActive);
        }

        private float GetCombinedAxis(string control, float keyboardValue)
        {
            return Mathf.Clamp(ApplyDeadzone(GetAxisOrButton(control)) + keyboardValue, -1f, 1f);
        }

        private float getFOVModification(float modifier)
        {
            float num = ((Camera.fieldOfView - 60f) / 60f + 1f) * modifier;
            if (Math.Abs(num) > float.Epsilon)
                return num;
            return 1f;
        }

        private float GetAxisOrButton(string control)
        {
            var gamepad = Gamepad.current;
            if (gamepad == null) return 0f;

            switch (control)
            {
                case LeftHorizontal: return gamepad.leftStick.x.ReadValue();
                case LeftVertical: return gamepad.leftStick.y.ReadValue();
                case RightHorizontal: return gamepad.rightStick.x.ReadValue();
                case RightVertical: return gamepad.rightStick.y.ReadValue();
                case LeftTrigger: return gamepad.leftTrigger.ReadValue();
                case RightTrigger: return gamepad.rightTrigger.ReadValue();
                case DPadLeft: return gamepad.dpad.left.isPressed ? 1f : 0f;
                case DPadRight: return gamepad.dpad.right.isPressed ? 1f : 0f;
                default: return 0f;
            }
        }

        private bool GetButton(string control)
        {
            var gamepad = Gamepad.current;
            if (gamepad == null) return false;

            switch (control)
            {
                case LeftBumper: return gamepad.leftShoulder.isPressed;
                case RightBumper: return gamepad.rightShoulder.isPressed;
                case DPadLeft: return gamepad.dpad.left.isPressed;
                case DPadRight: return gamepad.dpad.right.isPressed;
                default: return false;
            }
        }

        private bool GetButtonDown(string control)
        {
            var gamepad = Gamepad.current;
            if (gamepad == null) return false;

            switch (control)
            {
                case DPadUp: return gamepad.dpad.up.wasPressedThisFrame;
                case StartButton: return gamepad.startButton.wasPressedThisFrame;
                case BButton: return gamepad.buttonEast.wasPressedThisFrame;
                case XButton: return gamepad.buttonWest.wasPressedThisFrame;
                case YButton: return gamepad.buttonNorth.wasPressedThisFrame;
                default: return false;
            }
        }

        [Invokable("FreeCamera.SetControllerSpeeds", Description = "Adjusts the free camera controller speeds, sensitivities, and FOV modifiers.")]
        [PublicTweak]
        public static void SetControllerSpeeds(
            float xSpeed,
            float ySpeed,
            float zSpeed,
            float xSensitivity,
            float ySensitivity,
            float zSensitivity,
            float rotationModifierFOV,
            float xSpeedModifierFOV,
            float ySpeedModifierFOV,
            float zSpeedModifierFOV,
            float bumperRotationSensitivity)
        {
            PlayerPrefs.SetFloat(XSpeedPlayerPrefsKey, xSpeed);
            PlayerPrefs.SetFloat(YSpeedPlayerPrefsKey, ySpeed);
            PlayerPrefs.SetFloat(ZSpeedPlayerPrefsKey, zSpeed);
            PlayerPrefs.SetFloat(XSensitivityPlayerPrefsKey, xSensitivity);
            PlayerPrefs.SetFloat(YSensitivityPlayerPrefsKey, ySensitivity);
            PlayerPrefs.SetFloat(ZSensitivityPlayerPrefsKey, zSensitivity);
            PlayerPrefs.SetFloat(RotationModifierFOVPlayerPrefsKey, rotationModifierFOV);
            PlayerPrefs.SetFloat(XSpeedModifierFOVPlayerPrefsKey, xSpeedModifierFOV);
            PlayerPrefs.SetFloat(YSpeedModifierFOVPlayerPrefsKey, ySpeedModifierFOV);
            PlayerPrefs.SetFloat(ZSpeedModifierFOVPlayerPrefsKey, zSpeedModifierFOV);
            PlayerPrefs.SetFloat(BumperRotationSensitivityPlayerPrefsKey, bumperRotationSensitivity);
            PlayerPrefs.Save();

            Transform transform = Service.Get<GameObject>().transform.Find("FreeCameraTarget");
            if (transform != null)
            {
                FreeCameraController controller = transform.GetComponent<FreeCameraController>();
                if (controller != null)
                {
                    controller.XSpeed = xSpeed;
                    controller.YSpeed = ySpeed;
                    controller.ZSpeed = zSpeed;
                    controller.XSensitivity = xSensitivity;
                    controller.YSensitivity = ySensitivity;
                    controller.ZSensitivity = zSensitivity;
                    controller.RotationModifierFOV = rotationModifierFOV;
                    controller.XSpeedModifierFOV = xSpeedModifierFOV;
                    controller.YSpeedModifierFOV = ySpeedModifierFOV;
                    controller.ZSpeedModifierFOV = zSpeedModifierFOV;
                    controller.BumperRotationSensitivity = bumperRotationSensitivity;
                }
            }
        }

        [Invokable("FreeCamera.LoadDefaultSpeeds", Description = "Loads the default free camera controller speeds, sensitivities, and FOV modifiers.")]
        [PublicTweak]
        public static void LoadDefaultSpeeds()
        {
            SetControllerSpeeds(0.3f, 0.3f, 0.3f, 1f, 1f, 1f, 0.8f, 0.7f, 0.7f, 0.7f, 0.5f);
        }

        [Invokable("FreeCamera.StartCamera", Description = "Sets camera to free camera mode. Try plugging in a game controller. * This was used for in-game video capture")]
        [PublicTweak]
        public static void StartCamera()
        {
            Transform transform = Service.Get<GameObject>().transform;
            if (transform.Find("FreeCameraTarget") == null)
            {
                GameObject freeCameraTargetObj = new GameObject("FreeCameraTarget");
                freeCameraTargetObj.transform.SetParent(transform, false);
                freeCameraTargetObj.transform.position = Camera.main.transform.position;
                freeCameraTargetObj.transform.rotation = Camera.main.transform.rotation;
                FreeCameraController freeCameraController = freeCameraTargetObj.AddComponent<FreeCameraController>();
                freeCameraController.Target = freeCameraTargetObj.transform;
            }
        }

        [Invokable("FreeCamera.StopCamera", Description = "Stops free camera mode.")]
        [PublicTweak]
        public static void StopCamera()
        {
            Transform transform = Service.Get<GameObject>().transform.Find("FreeCameraTarget");
            if (transform != null)
                UnityEngine.Object.Destroy(transform.gameObject);
        }

        [Invokable("FreeCamera.ShowPlayer", Description = "Shows the local player while in free camera mode.")]
        [PublicTweak]
        public static void ShowPlayer()
        {
            Transform transform = Service.Get<GameObject>().transform.Find("FreeCameraTarget");
            if (transform != null)
            {
                FreeCameraController controller = transform.GetComponent<FreeCameraController>();
                if (controller != null)
                    controller.setLocalPlayerActive(true);
            }
        }

        [Invokable("FreeCamera.HidePlayer", Description = "Hides the local player while in free camera mode.")]
        [PublicTweak]
        public static void HidePlayer()
        {
            Transform transform = Service.Get<GameObject>().transform.Find("FreeCameraTarget");
            if (transform != null)
            {
                FreeCameraController controller = transform.GetComponent<FreeCameraController>();
                if (controller != null)
                    controller.setLocalPlayerActive(false);
            }
        }

        private void setLocalPlayerActive(bool isActive)
        {
            isLocalPlayerActive = isActive;
            if (localPlayerMask != 0 && localPlayerMask != -1)
            {
                if (isActive)
                    Camera.cullingMask |= 1 << localPlayerMask;
                else
                    Camera.cullingMask &= ~(1 << localPlayerMask);
            }
            if (blobShadowCaster != null)
                blobShadowCaster.SetIsActive(isActive);
        }

        private void OnDestroy()
        {
            if (context != null)
                context.SendEvent(new ExternalEvent("Root", "restoreUI"));

            Service.Get<EventDispatcher>().DispatchEvent(default(PlayerNameEvents.ShowPlayerNames));

            if (worldChatCanvas != null)
                worldChatCanvas.worldCamera = mainCamera;

            if (groupCulling != null)
                groupCulling.enabled = true;

            if (mainCamera != null)
                mainCamera.enabled = true;

            setLocalPlayerActive(true);
        }
    }
}