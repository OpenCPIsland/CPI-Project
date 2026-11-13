using UnityEngine;
using UnityEngine.InputSystem;

public class EasyTouchInput
{
    private Vector2[] oldMousePosition = new Vector2[2];
    private int[] tapCount = new int[2];
    private float[] startActionTime = new float[2];
    private float[] deltaTime = new float[2];
    private float[] tapeTime = new float[2];
    private bool bComplex = false;
    private Vector2 deltaFingerPosition;
    private Vector2 oldFinger2Position;
    private Vector2 complexCenter;

    public int TouchCount()
    {
        return getTouchCount(false);
    }

    private int getTouchCount(bool realTouch)
    {
        int result = 0;
        if (realTouch || EasyTouch.instance.enableRemote)
        {
            result = Touchscreen.current != null ? Touchscreen.current.touches.Count : 0;
        }
        else if (Mouse.current != null && (Mouse.current.leftButton.isPressed || Mouse.current.leftButton.wasReleasedThisFrame))
        {
            result = 1;
            if (IsAltPressed() || IsTwistKeyPressed() || IsCtrlPressed() || IsSwipeKeyPressed())
            {
                result = 2;
            }
            if (WasAltReleased() || WasTwistKeyReleased() || WasCtrlReleased() || WasSwipeKeyReleased())
            {
                result = 2;
            }
        }
        return result;
    }

    public Finger GetMouseTouch(int fingerIndex, Finger myFinger)
    {
        Finger finger = myFinger ?? new Finger() { gesture = EasyTouch.GestureType.None };

        if (fingerIndex == 1 && (WasAltReleased() || WasTwistKeyReleased() || WasCtrlReleased() || WasSwipeKeyReleased()))
        {
            finger.fingerIndex = fingerIndex;
            finger.position = oldFinger2Position;
            finger.deltaPosition = finger.position - oldFinger2Position;
            finger.tapCount = tapCount[fingerIndex];
            finger.deltaTime = Time.realtimeSinceStartup - deltaTime[fingerIndex];
            finger.phase = UnityEngine.TouchPhase.Ended;
            return finger;
        }

        if (Mouse.current != null && Mouse.current.leftButton.isPressed)
        {
            finger.fingerIndex = fingerIndex;
            finger.position = GetPointerPosition(fingerIndex);

            if ((double)(Time.realtimeSinceStartup - tapeTime[fingerIndex]) > 0.5)
            {
                tapCount[fingerIndex] = 0;
            }

            if (Mouse.current.leftButton.wasPressedThisFrame ||
                (fingerIndex == 1 && (WasAltPressedThisFrame() || WasTwistKeyPressedThisFrame() || WasCtrlPressedThisFrame() || WasSwipeKeyPressedThisFrame())))
            {
                finger.position = GetPointerPosition(fingerIndex);
                finger.deltaPosition = Vector2.zero;
                tapCount[fingerIndex]++;
                finger.tapCount = tapCount[fingerIndex];
                startActionTime[fingerIndex] = Time.realtimeSinceStartup;
                deltaTime[fingerIndex] = startActionTime[fingerIndex];
                finger.deltaTime = 0f;
                finger.phase = UnityEngine.TouchPhase.Began;
                if (fingerIndex == 1)
                    oldFinger2Position = finger.position;
                else
                    oldMousePosition[fingerIndex] = finger.position;

                if (tapCount[fingerIndex] == 1)
                    tapeTime[fingerIndex] = Time.realtimeSinceStartup;

                return finger;
            }

            finger.deltaPosition = finger.position - oldMousePosition[fingerIndex];
            finger.tapCount = tapCount[fingerIndex];
            finger.deltaTime = Time.realtimeSinceStartup - deltaTime[fingerIndex];
            finger.phase = finger.deltaPosition.sqrMagnitude < 1f ? UnityEngine.TouchPhase.Stationary : UnityEngine.TouchPhase.Moved;
            oldMousePosition[fingerIndex] = finger.position;
            deltaTime[fingerIndex] = Time.realtimeSinceStartup;
            return finger;
        }

        if (Mouse.current != null && Mouse.current.leftButton.wasReleasedThisFrame)
        {
            finger.fingerIndex = fingerIndex;
            finger.position = GetPointerPosition(fingerIndex);
            finger.deltaPosition = finger.position - oldMousePosition[fingerIndex];
            finger.tapCount = tapCount[fingerIndex];
            finger.deltaTime = Time.realtimeSinceStartup - deltaTime[fingerIndex];
            finger.phase = UnityEngine.TouchPhase.Ended;
            oldMousePosition[fingerIndex] = finger.position;
            return finger;
        }

        return null;
    }

    public Vector2 GetSecondFingerPosition()
    {
        Vector2 result = new Vector2(-1f, -1f);
        if ((IsAltPressed() || IsTwistKeyPressed()) && (IsCtrlPressed() || IsSwipeKeyPressed()))
        {
            if (!bComplex)
            {
                bComplex = true;
                deltaFingerPosition = GetMousePosition() - oldFinger2Position;
            }
            return GetComplex2finger();
        }
        if (IsAltPressed() || IsTwistKeyPressed())
        {
            result = GetPinchTwist2Finger();
            bComplex = false;
            return result;
        }
        if (IsCtrlPressed() || IsSwipeKeyPressed())
        {
            result = GetComplex2finger();
            bComplex = false;
            return result;
        }
        return result;
    }

    private Vector2 GetPointerPosition(int index)
    {
        if (index == 0)
        {
            return GetMousePosition();
        }
        return GetSecondFingerPosition();
    }

    private Vector2 GetPinchTwist2Finger()
    {
        Vector2 result;
        if (complexCenter == Vector2.zero)
        {
            result.x = (float)Screen.width / 2f - (GetMousePosition().x - (float)Screen.width / 2f);
            result.y = (float)Screen.height / 2f - (GetMousePosition().y - (float)Screen.height / 2f);
        }
        else
        {
            result.x = complexCenter.x - (GetMousePosition().x - complexCenter.x);
            result.y = complexCenter.y - (GetMousePosition().y - complexCenter.y);
        }
        oldFinger2Position = result;
        return result;
    }

    private Vector2 GetComplex2finger()
    {
        Vector2 result;
        result.x = GetMousePosition().x - deltaFingerPosition.x;
        result.y = GetMousePosition().y - deltaFingerPosition.y;
        complexCenter = new Vector2((GetMousePosition().x + result.x) / 2f, (GetMousePosition().y + result.y) / 2f);
        oldFinger2Position = result;
        return result;
    }

    private Vector2 GetMousePosition()
    {
        return Mouse.current != null ? Mouse.current.position.ReadValue() : Vector2.zero;
    }

    // Utility to convert KeyCode to Key for new Input System
    private UnityEngine.InputSystem.Key KeyCodeToKey(KeyCode kc)
    {
        switch (kc)
        {
            case KeyCode.None: return UnityEngine.InputSystem.Key.None;
            case KeyCode.LeftAlt: return UnityEngine.InputSystem.Key.LeftAlt;
            case KeyCode.LeftControl: return UnityEngine.InputSystem.Key.LeftCtrl;
            // Add more mappings as needed for your game!
            default: return UnityEngine.InputSystem.Key.None;
        }
    }

    private bool IsAltPressed() =>
        Keyboard.current != null && Keyboard.current.leftAltKey.isPressed;
    private bool IsCtrlPressed() =>
        Keyboard.current != null && Keyboard.current.leftCtrlKey.isPressed;
    private bool IsTwistKeyPressed() =>
        Keyboard.current != null && EasyTouch.instance.twistKey != KeyCode.None && Keyboard.current[KeyCodeToKey(EasyTouch.instance.twistKey)].isPressed;
    private bool IsSwipeKeyPressed() =>
        Keyboard.current != null && EasyTouch.instance.swipeKey != KeyCode.None && Keyboard.current[KeyCodeToKey(EasyTouch.instance.swipeKey)].isPressed;

    private bool WasAltReleased() =>
        Keyboard.current != null && Keyboard.current.leftAltKey.wasReleasedThisFrame;
    private bool WasCtrlReleased() =>
        Keyboard.current != null && Keyboard.current.leftCtrlKey.wasReleasedThisFrame;
    private bool WasTwistKeyReleased() =>
        Keyboard.current != null && EasyTouch.instance.twistKey != KeyCode.None && Keyboard.current[KeyCodeToKey(EasyTouch.instance.twistKey)].wasReleasedThisFrame;
    private bool WasSwipeKeyReleased() =>
        Keyboard.current != null && EasyTouch.instance.swipeKey != KeyCode.None && Keyboard.current[KeyCodeToKey(EasyTouch.instance.swipeKey)].wasReleasedThisFrame;

    private bool WasAltPressedThisFrame() =>
        Keyboard.current != null && Keyboard.current.leftAltKey.wasPressedThisFrame;
    private bool WasCtrlPressedThisFrame() =>
        Keyboard.current != null && Keyboard.current.leftCtrlKey.wasPressedThisFrame;
    private bool WasTwistKeyPressedThisFrame() =>
        Keyboard.current != null && EasyTouch.instance.twistKey != KeyCode.None && Keyboard.current[KeyCodeToKey(EasyTouch.instance.twistKey)].wasPressedThisFrame;
    private bool WasSwipeKeyPressedThisFrame() =>
        Keyboard.current != null && EasyTouch.instance.swipeKey != KeyCode.None && Keyboard.current[KeyCodeToKey(EasyTouch.instance.swipeKey)].wasPressedThisFrame;
}