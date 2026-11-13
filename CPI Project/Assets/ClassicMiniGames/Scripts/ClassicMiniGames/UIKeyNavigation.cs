using UnityEngine;
using UnityEngine.InputSystem;

[AddComponentMenu("NGUI/Interaction/Key Navigation")]
public class UIKeyNavigation : MonoBehaviour
{
    public enum Constraint
    {
        None,
        Vertical,
        Horizontal,
        Explicit
    }

    public static BetterList<UIKeyNavigation> list = new BetterList<UIKeyNavigation>();

    public Constraint constraint = Constraint.None;

    public GameObject onUp;
    public GameObject onDown;
    public GameObject onLeft;
    public GameObject onRight;
    public GameObject onClick;
    public bool startsSelected = false;

    protected virtual void OnEnable()
    {
        list.Add(this);
        if (startsSelected && (UICamera.selectedObject == null || !NGUITools.GetActive(UICamera.selectedObject)))
        {
            UICamera.currentScheme = UICamera.ControlScheme.Controller;
            UICamera.selectedObject = base.gameObject;
        }
    }

    protected virtual void OnDisable()
    {
        list.Remove(this);
    }

    protected GameObject GetLeft()
    {
        if (NGUITools.GetActive(onLeft))
        {
            return onLeft;
        }
        if (constraint == Constraint.Vertical || constraint == Constraint.Explicit)
        {
            return null;
        }
        return Get(Vector3.left, true);
    }

    private GameObject GetRight()
    {
        if (NGUITools.GetActive(onRight))
        {
            return onRight;
        }
        if (constraint == Constraint.Vertical || constraint == Constraint.Explicit)
        {
            return null;
        }
        return Get(Vector3.right, true);
    }

    protected GameObject GetUp()
    {
        if (NGUITools.GetActive(onUp))
        {
            return onUp;
        }
        if (constraint == Constraint.Horizontal || constraint == Constraint.Explicit)
        {
            return null;
        }
        return Get(Vector3.up, false);
    }

    protected GameObject GetDown()
    {
        if (NGUITools.GetActive(onDown))
        {
            return onDown;
        }
        if (constraint == Constraint.Horizontal || constraint == Constraint.Explicit)
        {
            return null;
        }
        return Get(Vector3.down, false);
    }

    protected GameObject Get(Vector3 myDir, bool horizontal)
    {
        Transform transform = base.transform;
        myDir = transform.TransformDirection(myDir);
        Vector3 center = GetCenter(base.gameObject);
        float num = float.MaxValue;
        GameObject result = null;
        for (int i = 0; i < list.size; i++)
        {
            UIKeyNavigation uIKeyNavigation = list[i];
            if (uIKeyNavigation == this)
            {
                continue;
            }
            UIButton component = uIKeyNavigation.GetComponent<UIButton>();
            if (component != null && !component.isEnabled)
            {
                continue;
            }
            Vector3 direction = GetCenter(uIKeyNavigation.gameObject) - center;
            float num2 = Vector3.Dot(myDir, direction.normalized);
            if (!(num2 < 0.707f))
            {
                direction = transform.InverseTransformDirection(direction);
                if (horizontal)
                {
                    direction.y *= 2f;
                }
                else
                {
                    direction.x *= 2f;
                }
                float sqrMagnitude = direction.sqrMagnitude;
                if (!(sqrMagnitude > num))
                {
                    result = uIKeyNavigation.gameObject;
                    num = sqrMagnitude;
                }
            }
        }
        return result;
    }

    protected static Vector3 GetCenter(GameObject go)
    {
        UIWidget component = go.GetComponent<UIWidget>();
        if (component != null)
        {
            Vector3[] worldCorners = component.worldCorners;
            return (worldCorners[0] + worldCorners[2]) * 0.5f;
        }
        return go.transform.position;
    }

    protected virtual void OnKey(Key key)
    {
        if (!NGUITools.GetActive(this))
        {
            return;
        }
        GameObject gameObject = null;
        switch (key)
        {
            case Key.LeftArrow:
                gameObject = GetLeft();
                break;
            case Key.RightArrow:
                gameObject = GetRight();
                break;
            case Key.UpArrow:
                gameObject = GetUp();
                break;
            case Key.DownArrow:
                gameObject = GetDown();
                break;
            case Key.Tab:
                if ((Keyboard.current != null && (Keyboard.current.leftShiftKey.isPressed || Keyboard.current.rightShiftKey.isPressed)))
                {
                    gameObject = GetLeft();
                    if (gameObject == null) gameObject = GetUp();
                    if (gameObject == null) gameObject = GetDown();
                    if (gameObject == null) gameObject = GetRight();
                }
                else
                {
                    gameObject = GetRight();
                    if (gameObject == null) gameObject = GetDown();
                    if (gameObject == null) gameObject = GetUp();
                    if (gameObject == null) gameObject = GetLeft();
                }
                break;
        }
        if (gameObject != null)
        {
            UICamera.selectedObject = gameObject;
        }
    }

    protected virtual void OnClick()
    {
        if (NGUITools.GetActive(this) && NGUITools.GetActive(onClick))
        {
            UICamera.selectedObject = onClick;
        }
    }

    // Example of invoking navigation with the new input system (call from Update or your input handler)
    void Update()
    {
        if (!NGUITools.GetActive(this)) return;
        if (UICamera.selectedObject != gameObject) return;
        if (Keyboard.current == null) return;

        if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
            OnKey(Key.LeftArrow);
        else if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
            OnKey(Key.RightArrow);
        else if (Keyboard.current.upArrowKey.wasPressedThisFrame)
            OnKey(Key.UpArrow);
        else if (Keyboard.current.downArrowKey.wasPressedThisFrame)
            OnKey(Key.DownArrow);
        else if (Keyboard.current.tabKey.wasPressedThisFrame)
            OnKey(Key.Tab);
        else if (Keyboard.current.enterKey.wasPressedThisFrame || Keyboard.current.numpadEnterKey.wasPressedThisFrame)
            OnClick();
    }
}