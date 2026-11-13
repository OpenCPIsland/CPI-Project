using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ClubPenguin.Input
{
	public static class ActiveInputDevice
	{
		public enum Kind
		{
			KeyboardMouse,
			Gamepad
		}

		public enum GamepadStyle
		{
			Xbox,
			PlayStation,
			Nintendo
		}

		public enum GamepadControl
		{
			ButtonSouth,
			ButtonEast,
			ButtonWest,
			ButtonNorth,
			LeftShoulder,
			RightShoulder,
			LeftTrigger,
			RightTrigger,
			DpadLeft,
			DpadRight,
			DpadUp,
			DpadDown,
			Start,
			Select
		}

		public static Kind CurrentKind { get; private set; } = Kind.KeyboardMouse;
		public static GamepadStyle CurrentGamepadStyle { get; private set; } = GamepadStyle.Xbox;

		public static event Action<Kind> OnChanged;

		private static Kind lastKind = Kind.KeyboardMouse;

		private const float StickDeadzone = 0.25f;
		private const float StickDeltaThreshold = 0.03f;
		private const float TriggerThreshold = 0.5f;

		private static int lastGamepadDeviceId = -1;
		private static Vector2 lastLeftStick = Vector2.zero;
		private static Vector2 lastRightStick = Vector2.zero;

		public static void Update()
		{
			Kind detected = DetectKindThisFrame();
			if (detected != lastKind)
			{
				lastKind = detected;
				CurrentKind = detected;
				OnChanged?.Invoke(CurrentKind);
			}
			else
			{
				CurrentKind = detected;
			}

			if (CurrentKind == Kind.Gamepad)
			{
				CurrentGamepadStyle = DetectGamepadStyle();
			}
		}

		private static Kind DetectKindThisFrame()
		{
			Keyboard kb = Keyboard.current;
			if (kb != null && kb.anyKey != null && kb.anyKey.wasPressedThisFrame)
			{
				return Kind.KeyboardMouse;
			}

			Mouse mouse = Mouse.current;
			if (mouse != null)
			{
				if (mouse.leftButton.wasPressedThisFrame ||
					mouse.rightButton.wasPressedThisFrame ||
					mouse.middleButton.wasPressedThisFrame ||
					mouse.scroll.ReadValue() != Vector2.zero ||
					mouse.delta.ReadValue().sqrMagnitude > 0.0f)
				{
					return Kind.KeyboardMouse;
				}
			}

			Gamepad gp = Gamepad.current;
			if (gp != null)
			{
				bool isNewGamepad = gp.deviceId != lastGamepadDeviceId;
				Vector2 ls = gp.leftStick.ReadValue();
				Vector2 rs = gp.rightStick.ReadValue();

				if (gp.leftTrigger.ReadValue() > TriggerThreshold || gp.rightTrigger.ReadValue() > TriggerThreshold)
				{
					lastGamepadDeviceId = gp.deviceId;
					lastLeftStick = ls;
					lastRightStick = rs;
					return Kind.Gamepad;
				}

				if (gp.buttonSouth.wasPressedThisFrame ||
					gp.buttonEast.wasPressedThisFrame ||
					gp.buttonWest.wasPressedThisFrame ||
					gp.buttonNorth.wasPressedThisFrame ||
					gp.leftShoulder.wasPressedThisFrame ||
					gp.rightShoulder.wasPressedThisFrame ||
					gp.startButton.wasPressedThisFrame ||
					gp.selectButton.wasPressedThisFrame ||
					gp.dpad.left.wasPressedThisFrame ||
					gp.dpad.right.wasPressedThisFrame ||
					gp.dpad.up.wasPressedThisFrame ||
					gp.dpad.down.wasPressedThisFrame)
				{
					lastGamepadDeviceId = gp.deviceId;
					lastLeftStick = ls;
					lastRightStick = rs;
					return Kind.Gamepad;
				}

				if (isNewGamepad)
				{
					lastGamepadDeviceId = gp.deviceId;
					lastLeftStick = ls;
					lastRightStick = rs;
					return lastKind;
				}

				Vector2 lsDelta = ls - lastLeftStick;
				Vector2 rsDelta = rs - lastRightStick;
				lastLeftStick = ls;
				lastRightStick = rs;

				bool lsMoved = ls.magnitude > StickDeadzone && lsDelta.magnitude > StickDeltaThreshold;
				bool rsMoved = rs.magnitude > StickDeadzone && rsDelta.magnitude > StickDeltaThreshold;
				if (lsMoved || rsMoved)
				{
					return Kind.Gamepad;
				}
			}

			return lastKind;
		}

		private static GamepadStyle DetectGamepadStyle()
		{
			Gamepad gp = Gamepad.current;
			if (gp == null)
			{
				return CurrentGamepadStyle;
			}

			string name = (gp.displayName ?? string.Empty).ToLowerInvariant();
			string manuf = gp.description.manufacturer.ToLowerInvariant();
			string product = gp.description.product.ToLowerInvariant();

			if (name.Contains("dualsense") || name.Contains("dualshock") || name.Contains("wireless controller") ||
				manuf.Contains("sony") || product.Contains("dualshock") || product.Contains("dualsense"))
			{
				return GamepadStyle.PlayStation;
			}

			if (name.Contains("switch") || name.Contains("nintendo") || manuf.Contains("nintendo") || product.Contains("nintendo"))
			{
				return GamepadStyle.Nintendo;
			}

			return GamepadStyle.Xbox;
		}

		public static string GetLabel(GamepadControl control)
		{
			switch (control)
			{
				case GamepadControl.ButtonSouth:
					return CurrentGamepadStyle == GamepadStyle.PlayStation ? "✕" :
						(CurrentGamepadStyle == GamepadStyle.Nintendo ? "B" : "A");
				case GamepadControl.ButtonEast:
					return CurrentGamepadStyle == GamepadStyle.PlayStation ? "○" :
						(CurrentGamepadStyle == GamepadStyle.Nintendo ? "A" : "B");
				case GamepadControl.ButtonWest:
					return CurrentGamepadStyle == GamepadStyle.PlayStation ? "□" :
						(CurrentGamepadStyle == GamepadStyle.Nintendo ? "Y" : "X");
				case GamepadControl.ButtonNorth:
					return CurrentGamepadStyle == GamepadStyle.PlayStation ? "△" :
						(CurrentGamepadStyle == GamepadStyle.Nintendo ? "X" : "Y");
				case GamepadControl.LeftShoulder:
					return CurrentGamepadStyle == GamepadStyle.PlayStation ? "L1" :
						(CurrentGamepadStyle == GamepadStyle.Nintendo ? "L" : "LB");
				case GamepadControl.RightShoulder:
					return CurrentGamepadStyle == GamepadStyle.PlayStation ? "R1" :
						(CurrentGamepadStyle == GamepadStyle.Nintendo ? "R" : "RB");
				case GamepadControl.LeftTrigger:
					return CurrentGamepadStyle == GamepadStyle.PlayStation ? "L2" :
						(CurrentGamepadStyle == GamepadStyle.Nintendo ? "ZL" : "LT");
				case GamepadControl.RightTrigger:
					return CurrentGamepadStyle == GamepadStyle.PlayStation ? "R2" :
						(CurrentGamepadStyle == GamepadStyle.Nintendo ? "ZR" : "RT");
				case GamepadControl.DpadLeft:
					return "D-Pad ◀";
				case GamepadControl.DpadRight:
					return "D-Pad ▶";
				case GamepadControl.DpadUp:
					return "D-Pad ▲";
				case GamepadControl.DpadDown:
					return "D-Pad ▼";
				case GamepadControl.Start:
					return CurrentGamepadStyle == GamepadStyle.PlayStation ? "Options" :
						(CurrentGamepadStyle == GamepadStyle.Nintendo ? "+" : "Menu");
				case GamepadControl.Select:
					return CurrentGamepadStyle == GamepadStyle.PlayStation ? "Share" :
						(CurrentGamepadStyle == GamepadStyle.Nintendo ? "-" : "View");
				default:
					return string.Empty;
			}
		}
	}
}
