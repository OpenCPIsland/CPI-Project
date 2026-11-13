using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[AddComponentMenu("Event/Extensions/GamePad Input Module")]
public class GamePadInputModule : BaseInputModule
{
    private float m_PrevActionTime;
    private Vector2 m_LastMoveVector;
    private int m_ConsecutiveMoveCount = 0;

    [SerializeField]
    private float m_InputActionsPerSecond = 10f;

    [SerializeField]
    private float m_RepeatDelay = 0.1f;

    public float inputActionsPerSecond
    {
        get { return m_InputActionsPerSecond; }
        set { m_InputActionsPerSecond = value; }
    }

    public float repeatDelay
    {
        get { return m_RepeatDelay; }
        set { m_RepeatDelay = value; }
    }

    protected GamePadInputModule() { }

    public override bool ShouldActivateModule()
    {
        if (!base.ShouldActivateModule())
            return false;

        bool flag = false;

        if (Gamepad.current != null)
        {
            flag |= Gamepad.current.buttonSouth.wasPressedThisFrame; // A/Cross = Submit
            flag |= Gamepad.current.buttonEast.wasPressedThisFrame; // B/Circle = Cancel
            flag |= !Mathf.Approximately(Gamepad.current.leftStick.ReadValue().x, 0f);
            flag |= !Mathf.Approximately(Gamepad.current.leftStick.ReadValue().y, 0f);
            flag |= Gamepad.current.dpad.up.wasPressedThisFrame ||
                    Gamepad.current.dpad.down.wasPressedThisFrame ||
                    Gamepad.current.dpad.left.wasPressedThisFrame ||
                    Gamepad.current.dpad.right.wasPressedThisFrame;
        }

        if (Keyboard.current != null)
        {
            flag |= Keyboard.current.enterKey.wasPressedThisFrame || Keyboard.current.spaceKey.wasPressedThisFrame; // Submit
            flag |= Keyboard.current.escapeKey.wasPressedThisFrame; // Cancel
            flag |= Keyboard.current.leftArrowKey.isPressed || Keyboard.current.rightArrowKey.isPressed;
            flag |= Keyboard.current.upArrowKey.isPressed || Keyboard.current.downArrowKey.isPressed;
            flag |= Keyboard.current.aKey.isPressed || Keyboard.current.dKey.isPressed;
            flag |= Keyboard.current.wKey.isPressed || Keyboard.current.sKey.isPressed;
        }

        return flag;
    }

    public override void ActivateModule()
    {
        StandaloneInputModule component = GetComponent<StandaloneInputModule>();
        if (component && component.enabled)
        {
            Debug.LogError("StandAloneInputSystem should not be used with the GamePadInputModule, please remove it from the Event System in this scene or disable it when this module is in use");
        }
        base.ActivateModule();
        GameObject go = eventSystem.currentSelectedGameObject ?? eventSystem.firstSelectedGameObject;
        eventSystem.SetSelectedGameObject(go, GetBaseEventData());
    }

    public override void DeactivateModule()
    {
        base.DeactivateModule();
    }

    public override void Process()
    {
        bool usedEvent = SendUpdateEventToSelectedObject();
        if (eventSystem.sendNavigationEvents)
        {
            if (!usedEvent)
                usedEvent |= SendMoveEventToSelectedObject();
            if (!usedEvent)
                SendSubmitEventToSelectedObject();
        }
    }

    protected bool SendSubmitEventToSelectedObject()
    {
        if (eventSystem.currentSelectedGameObject == null)
            return false;

        BaseEventData data = GetBaseEventData();
        bool used = false;

        // Gamepad submit/cancel
        if (Gamepad.current != null)
        {
            if (Gamepad.current.buttonSouth.wasPressedThisFrame) // A/Cross
            {
                ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, data, ExecuteEvents.submitHandler);
                used = true;
            }
            if (Gamepad.current.buttonEast.wasPressedThisFrame) // B/Circle
            {
                ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, data, ExecuteEvents.cancelHandler);
                used = true;
            }
        }

        // Keyboard submit/cancel
        if (Keyboard.current != null)
        {
            if (Keyboard.current.enterKey.wasPressedThisFrame || Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, data, ExecuteEvents.submitHandler);
                used = true;
            }
            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, data, ExecuteEvents.cancelHandler);
                used = true;
            }
        }

        return used || data.used;
    }

    private Vector2 GetRawMoveVector()
    {
        Vector2 move = Vector2.zero;

        // Gamepad stick/dpad
        if (Gamepad.current != null)
        {
            Vector2 stick = Gamepad.current.leftStick.ReadValue();
            move.x = Mathf.Abs(stick.x) > 0.5f ? Mathf.Sign(stick.x) : 0f;
            move.y = Mathf.Abs(stick.y) > 0.5f ? Mathf.Sign(stick.y) : 0f;

            if (Gamepad.current.dpad.left.isPressed) move.x = -1f;
            if (Gamepad.current.dpad.right.isPressed) move.x = 1f;
            if (Gamepad.current.dpad.up.isPressed) move.y = 1f;
            if (Gamepad.current.dpad.down.isPressed) move.y = -1f;
        }

        // Keyboard arrows/WASD
        if (Keyboard.current != null)
        {
            if (Keyboard.current.leftArrowKey.isPressed || Keyboard.current.aKey.isPressed) move.x = -1f;
            if (Keyboard.current.rightArrowKey.isPressed || Keyboard.current.dKey.isPressed) move.x = 1f;
            if (Keyboard.current.upArrowKey.isPressed || Keyboard.current.wKey.isPressed) move.y = 1f;
            if (Keyboard.current.downArrowKey.isPressed || Keyboard.current.sKey.isPressed) move.y = -1f;
        }

        return move;
    }

    protected bool SendMoveEventToSelectedObject()
    {
        float unscaledTime = Time.unscaledTime;
        Vector2 move = GetRawMoveVector();
        if (Mathf.Approximately(move.x, 0f) && Mathf.Approximately(move.y, 0f))
        {
            m_ConsecutiveMoveCount = 0;
            return false;
        }

        bool pressed = move != m_LastMoveVector;
        bool sameDir = Vector2.Dot(move, m_LastMoveVector) > 0f;

        if (!pressed)
        {
            pressed = (!sameDir || m_ConsecutiveMoveCount != 1)
                ? (unscaledTime > m_PrevActionTime + 1f / m_InputActionsPerSecond)
                : (unscaledTime > m_PrevActionTime + m_RepeatDelay);
        }

        if (!pressed)
            return false;

        AxisEventData axisData = GetAxisEventData(move.x, move.y, 0.6f);
        ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, axisData, ExecuteEvents.moveHandler);

        if (!sameDir)
            m_ConsecutiveMoveCount = 0;

        m_ConsecutiveMoveCount++;
        m_PrevActionTime = unscaledTime;
        m_LastMoveVector = move;

        return axisData.used;
    }

    protected bool SendUpdateEventToSelectedObject()
    {
        if (eventSystem.currentSelectedGameObject == null)
            return false;
        BaseEventData data = GetBaseEventData();
        ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, data, ExecuteEvents.updateSelectedHandler);
        return data.used;
    }
}