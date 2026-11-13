using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace ClubPenguin.Input
{
	public class KeyCodeInput : Input<ButtonInputResult>
	{
		public KeyCode[] Keys = new KeyCode[0];

		[NonSerialized]
		private KeyCode[] mutableKeys;

		[NonSerialized]
		private ActiveInputDevice.GamepadControl[] gamepadControls = Array.Empty<ActiveInputDevice.GamepadControl>();

		[NonSerialized]
		private bool[] gamepadHeldPrev = Array.Empty<bool>();

		private const float TriggerThreshold = 0.5f;

		public KeyCode PrimaryKey
		{
			get
			{
				return (mutableKeys != null && mutableKeys.Length != 0) ? mutableKeys[0] : KeyCode.None;
			}
		}

		public override void Initialize(KeyCodeRemapper keyCodeRemapper)
		{
			mutableKeys = new KeyCode[Keys.Length];
			for (int i = 0; i < Keys.Length; i++)
			{
				mutableKeys[i] = keyCodeRemapper.GetKeyCode(Keys[i]);
			}
			base.Initialize(keyCodeRemapper);
		}

		public void SetGamepadBindings(params ActiveInputDevice.GamepadControl[] controls)
		{
			gamepadControls = controls ?? Array.Empty<ActiveInputDevice.GamepadControl>();
			gamepadHeldPrev = new bool[gamepadControls.Length];
		}

		protected override bool process(int filter)
		{
			bool isPressed = false;
			bool wasPressedThisFrame = false;

			if (mutableKeys != null)
			{
				for (int i = 0; i < mutableKeys.Length; i++)
				{
					ButtonControl control = GetButtonControl(mutableKeys[i]);
					if (control != null)
					{
						wasPressedThisFrame |= control.wasPressedThisFrame;
						isPressed |= control.isPressed;
					}
				}
			}

			Gamepad gp = Gamepad.current;
			if (gp != null && gamepadControls != null && gamepadControls.Length > 0)
			{
				bool allowGamepad = ActiveInputDevice.CurrentKind == ActiveInputDevice.Kind.Gamepad;
				if (!allowGamepad)
				{
					for (int i = 0; i < gamepadControls.Length; i++)
					{
						if (IsGamepadControlActivatedThisFrame(gp, gamepadControls[i]))
						{
							allowGamepad = true;
							break;
						}
					}
				}

				if (allowGamepad)
				{
					for (int i = 0; i < gamepadControls.Length; i++)
					{
						bool held = IsGamepadControlHeld(gp, gamepadControls[i]);
						bool prev = gamepadHeldPrev[i];

						isPressed |= held;
						wasPressedThisFrame |= (held && !prev);

						gamepadHeldPrev[i] = held;
					}
				}
				else
				{
					for (int i = 0; i < gamepadHeldPrev.Length; i++)
					{
						gamepadHeldPrev[i] = false;
					}
				}
			}

			inputEvent.WasJustPressed = (wasPressedThisFrame && !inputEvent.IsHeld);
			inputEvent.WasJustReleased = (!isPressed && inputEvent.IsHeld);
			inputEvent.IsHeld = (isPressed || wasPressedThisFrame);

			return inputEvent.IsHeld || inputEvent.WasJustReleased;
		}

		private static bool IsGamepadControlHeld(Gamepad gp, ActiveInputDevice.GamepadControl control)
		{
			switch (control)
			{
				case ActiveInputDevice.GamepadControl.ButtonSouth: return gp.buttonSouth.isPressed;
				case ActiveInputDevice.GamepadControl.ButtonEast: return gp.buttonEast.isPressed;
				case ActiveInputDevice.GamepadControl.ButtonWest: return gp.buttonWest.isPressed;
				case ActiveInputDevice.GamepadControl.ButtonNorth: return gp.buttonNorth.isPressed;

				case ActiveInputDevice.GamepadControl.LeftShoulder: return gp.leftShoulder.isPressed;
				case ActiveInputDevice.GamepadControl.RightShoulder: return gp.rightShoulder.isPressed;

				case ActiveInputDevice.GamepadControl.LeftTrigger: return gp.leftTrigger.ReadValue() > TriggerThreshold;
				case ActiveInputDevice.GamepadControl.RightTrigger: return gp.rightTrigger.ReadValue() > TriggerThreshold;

				case ActiveInputDevice.GamepadControl.DpadLeft: return gp.dpad.left.isPressed;
				case ActiveInputDevice.GamepadControl.DpadRight: return gp.dpad.right.isPressed;
				case ActiveInputDevice.GamepadControl.DpadUp: return gp.dpad.up.isPressed;
				case ActiveInputDevice.GamepadControl.DpadDown: return gp.dpad.down.isPressed;

				case ActiveInputDevice.GamepadControl.Start: return gp.startButton.isPressed;
				case ActiveInputDevice.GamepadControl.Select: return gp.selectButton.isPressed;

				default: return false;
			}
		}

		private static bool IsGamepadControlActivatedThisFrame(Gamepad gp, ActiveInputDevice.GamepadControl control)
		{
			switch (control)
			{
				case ActiveInputDevice.GamepadControl.ButtonSouth: return gp.buttonSouth.wasPressedThisFrame;
				case ActiveInputDevice.GamepadControl.ButtonEast: return gp.buttonEast.wasPressedThisFrame;
				case ActiveInputDevice.GamepadControl.ButtonWest: return gp.buttonWest.wasPressedThisFrame;
				case ActiveInputDevice.GamepadControl.ButtonNorth: return gp.buttonNorth.wasPressedThisFrame;

				case ActiveInputDevice.GamepadControl.LeftShoulder: return gp.leftShoulder.wasPressedThisFrame;
				case ActiveInputDevice.GamepadControl.RightShoulder: return gp.rightShoulder.wasPressedThisFrame;

				case ActiveInputDevice.GamepadControl.LeftTrigger: return gp.leftTrigger.ReadValue() > TriggerThreshold;
				case ActiveInputDevice.GamepadControl.RightTrigger: return gp.rightTrigger.ReadValue() > TriggerThreshold;

				case ActiveInputDevice.GamepadControl.DpadLeft: return gp.dpad.left.wasPressedThisFrame;
				case ActiveInputDevice.GamepadControl.DpadRight: return gp.dpad.right.wasPressedThisFrame;
				case ActiveInputDevice.GamepadControl.DpadUp: return gp.dpad.up.wasPressedThisFrame;
				case ActiveInputDevice.GamepadControl.DpadDown: return gp.dpad.down.wasPressedThisFrame;

				case ActiveInputDevice.GamepadControl.Start: return gp.startButton.wasPressedThisFrame;
				case ActiveInputDevice.GamepadControl.Select: return gp.selectButton.wasPressedThisFrame;
			}
			return false;
		}

		private static ButtonControl GetButtonControl(KeyCode keyCode)
		{
			Keyboard kb = Keyboard.current;
			Mouse mouse = Mouse.current;

			if (kb == null && mouse == null)
			{
				return null;
			}

			switch (keyCode)
			{
				case KeyCode.A: return kb?.aKey;
				case KeyCode.B: return kb?.bKey;
				case KeyCode.C: return kb?.cKey;
				case KeyCode.D: return kb?.dKey;
				case KeyCode.E: return kb?.eKey;
				case KeyCode.F: return kb?.fKey;
				case KeyCode.G: return kb?.gKey;
				case KeyCode.H: return kb?.hKey;
				case KeyCode.I: return kb?.iKey;
				case KeyCode.J: return kb?.jKey;
				case KeyCode.K: return kb?.kKey;
				case KeyCode.L: return kb?.lKey;
				case KeyCode.M: return kb?.mKey;
				case KeyCode.N: return kb?.nKey;
				case KeyCode.O: return kb?.oKey;
				case KeyCode.P: return kb?.pKey;
				case KeyCode.Q: return kb?.qKey;
				case KeyCode.R: return kb?.rKey;
				case KeyCode.S: return kb?.sKey;
				case KeyCode.T: return kb?.tKey;
				case KeyCode.U: return kb?.uKey;
				case KeyCode.V: return kb?.vKey;
				case KeyCode.W: return kb?.wKey;
				case KeyCode.X: return kb?.xKey;
				case KeyCode.Y: return kb?.yKey;
				case KeyCode.Z: return kb?.zKey;

				case KeyCode.Alpha0: return kb?.digit0Key;
				case KeyCode.Alpha1: return kb?.digit1Key;
				case KeyCode.Alpha2: return kb?.digit2Key;
				case KeyCode.Alpha3: return kb?.digit3Key;
				case KeyCode.Alpha4: return kb?.digit4Key;
				case KeyCode.Alpha5: return kb?.digit5Key;
				case KeyCode.Alpha6: return kb?.digit6Key;
				case KeyCode.Alpha7: return kb?.digit7Key;
				case KeyCode.Alpha8: return kb?.digit8Key;
				case KeyCode.Alpha9: return kb?.digit9Key;

				case KeyCode.Space: return kb?.spaceKey;
				case KeyCode.Return: return kb?.enterKey;
				case KeyCode.KeypadEnter: return kb?.numpadEnterKey;
				case KeyCode.Escape: return kb?.escapeKey;
				case KeyCode.Tab: return kb?.tabKey;
				case KeyCode.Backspace: return kb?.backspaceKey;
				case KeyCode.Delete: return kb?.deleteKey;
				case KeyCode.Insert: return kb?.insertKey;
				case KeyCode.Home: return kb?.homeKey;
				case KeyCode.End: return kb?.endKey;
				case KeyCode.PageUp: return kb?.pageUpKey;
				case KeyCode.PageDown: return kb?.pageDownKey;

				case KeyCode.BackQuote: return kb?.backquoteKey;
				case KeyCode.Minus: return kb?.minusKey;
				case KeyCode.Equals: return kb?.equalsKey;
				case KeyCode.LeftBracket: return kb?.leftBracketKey;
				case KeyCode.RightBracket: return kb?.rightBracketKey;
				case KeyCode.Backslash: return kb?.backslashKey;
				case KeyCode.Semicolon: return kb?.semicolonKey;
				case KeyCode.Quote: return kb?.quoteKey;
				case KeyCode.Comma: return kb?.commaKey;
				case KeyCode.Period: return kb?.periodKey;
				case KeyCode.Slash: return kb?.slashKey;

				case KeyCode.LeftShift: return kb?.leftShiftKey;
				case KeyCode.RightShift: return kb?.rightShiftKey;
				case KeyCode.LeftControl: return kb?.leftCtrlKey;
				case KeyCode.RightControl: return kb?.rightCtrlKey;
				case KeyCode.LeftAlt: return kb?.leftAltKey;
				case KeyCode.RightAlt: return kb?.rightAltKey;
				case KeyCode.LeftCommand: return kb?.leftMetaKey;
				case KeyCode.RightCommand: return kb?.rightMetaKey;
				case KeyCode.CapsLock: return kb?.capsLockKey;

				case KeyCode.UpArrow: return kb?.upArrowKey;
				case KeyCode.DownArrow: return kb?.downArrowKey;
				case KeyCode.LeftArrow: return kb?.leftArrowKey;
				case KeyCode.RightArrow: return kb?.rightArrowKey;

				case KeyCode.F1: return kb?.f1Key;
				case KeyCode.F2: return kb?.f2Key;
				case KeyCode.F3: return kb?.f3Key;
				case KeyCode.F4: return kb?.f4Key;
				case KeyCode.F5: return kb?.f5Key;
				case KeyCode.F6: return kb?.f6Key;
				case KeyCode.F7: return kb?.f7Key;
				case KeyCode.F8: return kb?.f8Key;
				case KeyCode.F9: return kb?.f9Key;
				case KeyCode.F10: return kb?.f10Key;
				case KeyCode.F11: return kb?.f11Key;
				case KeyCode.F12: return kb?.f12Key;

				case KeyCode.Mouse0: return mouse?.leftButton;
				case KeyCode.Mouse1: return mouse?.rightButton;
				case KeyCode.Mouse2: return mouse?.middleButton;
				case KeyCode.Mouse3: return mouse?.forwardButton;
				case KeyCode.Mouse4: return mouse?.backButton;

				default:
					return null;
			}
		}
	}
}
