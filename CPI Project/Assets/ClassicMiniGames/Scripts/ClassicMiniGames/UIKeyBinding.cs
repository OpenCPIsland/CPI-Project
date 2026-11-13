using UnityEngine;
using UnityEngine.InputSystem;

[AddComponentMenu("NGUI/Interaction/Key Binding")]
public class UIKeyBinding : MonoBehaviour
{
    public enum Action
    {
        PressAndClick,
        Select
    }

    public enum Modifier
    {
        None,
        Shift,
        Control,
        Alt
    }

    public KeyCode keyCode = KeyCode.None;

    public Modifier modifier = Modifier.None;

    public Action action = Action.PressAndClick;

    private bool mIgnoreUp = false;

    private bool mIsInput = false;

    private void Start()
    {
        UIInput component = GetComponent<UIInput>();
        mIsInput = (component != null);
        if (component != null)
        {
            EventDelegate.Add(component.onSubmit, OnSubmit);
        }
    }

    private void OnSubmit()
    {
        if (UICamera.currentKey == keyCode && IsModifierActive())
        {
            mIgnoreUp = true;
        }
    }

    private bool IsModifierActive()
    {
        if (modifier == Modifier.None)
        {
            return true;
        }
        if (modifier == Modifier.Alt)
        {
            if ((Keyboard.current != null && Keyboard.current.leftAltKey.isPressed) || (Keyboard.current != null && Keyboard.current.rightAltKey.isPressed))
            {
                return true;
            }
        }
        else if (modifier == Modifier.Control)
        {
            if ((Keyboard.current != null && Keyboard.current.leftCtrlKey.isPressed) || (Keyboard.current != null && Keyboard.current.rightCtrlKey.isPressed))
            {
                return true;
            }
        }
        else if (modifier == Modifier.Shift)
        {
            if ((Keyboard.current != null && Keyboard.current.leftShiftKey.isPressed) || (Keyboard.current != null && Keyboard.current.rightShiftKey.isPressed))
            {
                return true;
            }
        }
        return false;
    }

    private bool IsKeyDown(KeyCode key)
    {
        if (Keyboard.current == null) return false;
        var k = ToInputSystemKey(key);
        if (k == Key.None) return false;
        return Keyboard.current[k].wasPressedThisFrame;
    }

    private bool IsKeyUp(KeyCode key)
    {
        if (Keyboard.current == null) return false;
        var k = ToInputSystemKey(key);
        if (k == Key.None) return false;
        return Keyboard.current[k].wasReleasedThisFrame;
    }

    private void Update()
    {
        if (keyCode == KeyCode.None || !IsModifierActive())
        {
            return;
        }
        if (action == Action.PressAndClick)
        {
            if (!UICamera.inputHasFocus)
            {
                UICamera.currentTouch = UICamera.controller;
                UICamera.currentScheme = UICamera.ControlScheme.Mouse;
                UICamera.currentTouch.current = gameObject;
                if (IsKeyDown(keyCode))
                {
                    UICamera.Notify(gameObject, "OnPress", true);
                }
                if (IsKeyUp(keyCode))
                {
                    UICamera.Notify(gameObject, "OnPress", false);
                    UICamera.Notify(gameObject, "OnClick", null);
                }
                UICamera.currentTouch.current = null;
            }
        }
        else
        {
            if (action != Action.Select || !IsKeyUp(keyCode))
            {
                return;
            }
            if (mIsInput)
            {
                if (!mIgnoreUp && !UICamera.inputHasFocus)
                {
                    UICamera.selectedObject = gameObject;
                }
                mIgnoreUp = false;
            }
            else
            {
                UICamera.selectedObject = gameObject;
            }
        }
    }

    private Key ToInputSystemKey(KeyCode keyCode)
    {
        switch (keyCode)
        {
            case KeyCode.A: return Key.A;
            case KeyCode.B: return Key.B;
            case KeyCode.C: return Key.C;
            case KeyCode.D: return Key.D;
            case KeyCode.E: return Key.E;
            case KeyCode.F: return Key.F;
            case KeyCode.G: return Key.G;
            case KeyCode.H: return Key.H;
            case KeyCode.I: return Key.I;
            case KeyCode.J: return Key.J;
            case KeyCode.K: return Key.K;
            case KeyCode.L: return Key.L;
            case KeyCode.M: return Key.M;
            case KeyCode.N: return Key.N;
            case KeyCode.O: return Key.O;
            case KeyCode.P: return Key.P;
            case KeyCode.Q: return Key.Q;
            case KeyCode.R: return Key.R;
            case KeyCode.S: return Key.S;
            case KeyCode.T: return Key.T;
            case KeyCode.U: return Key.U;
            case KeyCode.V: return Key.V;
            case KeyCode.W: return Key.W;
            case KeyCode.X: return Key.X;
            case KeyCode.Y: return Key.Y;
            case KeyCode.Z: return Key.Z;
            case KeyCode.Alpha0: return Key.Digit0;
            case KeyCode.Alpha1: return Key.Digit1;
            case KeyCode.Alpha2: return Key.Digit2;
            case KeyCode.Alpha3: return Key.Digit3;
            case KeyCode.Alpha4: return Key.Digit4;
            case KeyCode.Alpha5: return Key.Digit5;
            case KeyCode.Alpha6: return Key.Digit6;
            case KeyCode.Alpha7: return Key.Digit7;
            case KeyCode.Alpha8: return Key.Digit8;
            case KeyCode.Alpha9: return Key.Digit9;
            case KeyCode.Escape: return Key.Escape;
            case KeyCode.Return: return Key.Enter;
            case KeyCode.Space: return Key.Space;
            case KeyCode.LeftShift: return Key.LeftShift;
            case KeyCode.RightShift: return Key.RightShift;
            case KeyCode.LeftAlt: return Key.LeftAlt;
            case KeyCode.RightAlt: return Key.RightAlt;
            case KeyCode.LeftControl: return Key.LeftCtrl;
            case KeyCode.RightControl: return Key.RightCtrl;
            case KeyCode.Tab: return Key.Tab;
            case KeyCode.UpArrow: return Key.UpArrow;
            case KeyCode.DownArrow: return Key.DownArrow;
            case KeyCode.LeftArrow: return Key.LeftArrow;
            case KeyCode.RightArrow: return Key.RightArrow;
            // Add additional mappings as needed
            default: return Key.None;
        }
    }
}