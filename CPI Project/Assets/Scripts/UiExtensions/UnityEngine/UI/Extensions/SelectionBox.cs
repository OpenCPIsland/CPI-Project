using System;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace UnityEngine.UI.Extensions
{
    [RequireComponent(typeof(Canvas))]
    [AddComponentMenu("UI/Extensions/Selection Box")]
    public class SelectionBox : MonoBehaviour
    {
        public class SelectionEvent : UnityEvent<IBoxSelectable[]>
        {
        }

        public Color color;
        public Sprite art;

        private Vector2 origin;
        public RectTransform selectionMask;
        private RectTransform boxRect;
        private IBoxSelectable[] selectables;
        private MonoBehaviour[] selectableGroup;
        private IBoxSelectable clickedBeforeDrag;
        private IBoxSelectable clickedAfterDrag;

        public SelectionEvent onSelectionChange = new SelectionEvent();

        private void ValidateCanvas()
        {
            Canvas component = gameObject.GetComponent<Canvas>();
            if (component.renderMode != 0)
            {
                throw new Exception("SelectionBox component must be placed on a canvas in Screen Space Overlay mode.");
            }
            CanvasScaler component2 = gameObject.GetComponent<CanvasScaler>();
            if (component2 && component2.enabled && (!Mathf.Approximately(component2.scaleFactor, 1f) || component2.uiScaleMode != 0))
            {
                Destroy(component2);
                Debug.LogWarning("SelectionBox component is on a gameObject with a Canvas Scaler component. As of now, Canvas Scalers without the default settings throw off the coordinates of the selection box. Canvas Scaler has been removed.");
            }
        }

        private void SetSelectableGroup(IEnumerable<MonoBehaviour> behaviourCollection)
        {
            if (behaviourCollection == null)
            {
                selectableGroup = null;
                return;
            }
            List<MonoBehaviour> list = new List<MonoBehaviour>();
            foreach (MonoBehaviour item in behaviourCollection)
            {
                if (item is IBoxSelectable)
                {
                    list.Add(item);
                }
            }
            selectableGroup = list.ToArray();
        }

        private void CreateBoxRect()
        {
            GameObject go = new GameObject();
            go.name = "Selection Box";
            go.transform.parent = transform;
            go.AddComponent<Image>();
            boxRect = go.transform as RectTransform;
        }

        private void ResetBoxRect()
        {
            Image component = boxRect.GetComponent<Image>();
            component.color = color;
            component.sprite = art;
            origin = Vector2.zero;
            boxRect.anchoredPosition = Vector2.zero;
            boxRect.sizeDelta = Vector2.zero;
            boxRect.anchorMax = Vector2.zero;
            boxRect.anchorMin = Vector2.zero;
            boxRect.pivot = Vector2.zero;
            boxRect.gameObject.SetActive(false);
        }

        private void BeginSelection()
        {
            // Use new Input System
            if (Mouse.current == null || !Mouse.current.leftButton.wasPressedThisFrame)
            {
                return;
            }
            boxRect.gameObject.SetActive(true);
            Vector3 mousePosition = Mouse.current.position.ReadValue();
            float x = mousePosition.x;
            float y = mousePosition.y;
            origin = new Vector2(x, y);
            if (!PointIsValidAgainstSelectionMask(origin))
            {
                ResetBoxRect();
                return;
            }
            boxRect.anchoredPosition = origin;
            MonoBehaviour[] array = (selectableGroup != null) ? selectableGroup : FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);
            List<IBoxSelectable> list = new List<IBoxSelectable>();
            foreach (MonoBehaviour monoBehaviour in array)
            {
                IBoxSelectable boxSelectable = monoBehaviour as IBoxSelectable;
                if (boxSelectable != null)
                {
                    list.Add(boxSelectable);
                    // Use Keyboard.current for Shift
                    bool shiftHeld = Keyboard.current != null &&
                        (Keyboard.current[Key.LeftShift].isPressed || Keyboard.current[Key.RightShift].isPressed);
                    if (!shiftHeld)
                    {
                        boxSelectable.selected = false;
                    }
                }
            }
            selectables = list.ToArray();
            clickedBeforeDrag = GetSelectableAtMousePosition();
        }

        private bool PointIsValidAgainstSelectionMask(Vector2 screenPoint)
        {
            if (!selectionMask)
            {
                return true;
            }
            Camera screenPointCamera = GetScreenPointCamera(selectionMask);
            return RectTransformUtility.RectangleContainsScreenPoint(selectionMask, screenPoint, screenPointCamera);
        }

        private IBoxSelectable GetSelectableAtMousePosition()
        {
            Vector2 mousePosition = Mouse.current != null ? Mouse.current.position.ReadValue() : Vector2.zero;
            if (!PointIsValidAgainstSelectionMask(mousePosition))
            {
                return null;
            }
            IBoxSelectable[] array = selectables;
            foreach (IBoxSelectable boxSelectable in array)
            {
                RectTransform rectTransform = boxSelectable.transform as RectTransform;
                if (rectTransform)
                {
                    Camera screenPointCamera = GetScreenPointCamera(rectTransform);
                    if (RectTransformUtility.RectangleContainsScreenPoint(rectTransform, mousePosition, screenPointCamera))
                    {
                        return boxSelectable;
                    }
                    continue;
                }
                float magnitude = boxSelectable.transform.GetComponent<Renderer>().bounds.extents.magnitude;
                Vector2 screenPointOfSelectable = GetScreenPointOfSelectable(boxSelectable);
                if (Vector2.Distance(screenPointOfSelectable, mousePosition) <= magnitude)
                {
                    return boxSelectable;
                }
            }
            return null;
        }

        private void DragSelection()
        {
            if (Mouse.current != null && Mouse.current.leftButton.isPressed && boxRect.gameObject.activeSelf)
            {
                Vector3 mousePosition = Mouse.current.position.ReadValue();
                float x = mousePosition.x;
                float y = mousePosition.y;
                Vector2 a = new Vector2(x, y);
                Vector2 sizeDelta = a - origin;
                Vector2 anchoredPosition = origin;
                if (sizeDelta.x < 0f)
                {
                    anchoredPosition.x = a.x;
                    sizeDelta.x = -sizeDelta.x;
                }
                if (sizeDelta.y < 0f)
                {
                    anchoredPosition.y = a.y;
                    sizeDelta.y = -sizeDelta.y;
                }
                boxRect.anchoredPosition = anchoredPosition;
                boxRect.sizeDelta = sizeDelta;
                IBoxSelectable[] array = selectables;
                foreach (IBoxSelectable boxSelectable in array)
                {
                    Vector3 v = GetScreenPointOfSelectable(boxSelectable);
                    boxSelectable.preSelected = (RectTransformUtility.RectangleContainsScreenPoint(boxRect, v, null) && PointIsValidAgainstSelectionMask(v));
                }
                if (clickedBeforeDrag != null)
                {
                    clickedBeforeDrag.preSelected = true;
                }
            }
        }

        private void ApplySingleClickDeselection()
        {
            if (clickedBeforeDrag != null && clickedAfterDrag != null && clickedBeforeDrag.selected && clickedBeforeDrag.transform == clickedAfterDrag.transform)
            {
                clickedBeforeDrag.selected = false;
                clickedBeforeDrag.preSelected = false;
            }
        }

        private void ApplyPreSelections()
        {
            IBoxSelectable[] array = selectables;
            foreach (IBoxSelectable boxSelectable in array)
            {
                if (boxSelectable.preSelected)
                {
                    boxSelectable.selected = true;
                    boxSelectable.preSelected = false;
                }
            }
        }

        private Vector2 GetScreenPointOfSelectable(IBoxSelectable selectable)
        {
            RectTransform rectTransform = selectable.transform as RectTransform;
            if (rectTransform)
            {
                Camera screenPointCamera = GetScreenPointCamera(rectTransform);
                return RectTransformUtility.WorldToScreenPoint(screenPointCamera, selectable.transform.position);
            }
            return Camera.main.WorldToScreenPoint(selectable.transform.position);
        }

        private Camera GetScreenPointCamera(RectTransform rectTransform)
        {
            Canvas canvas = null;
            RectTransform rectTransform2 = rectTransform;
            do
            {
                canvas = rectTransform2.GetComponent<Canvas>();
                if (canvas && !canvas.isRootCanvas)
                {
                    canvas = null;
                }
                rectTransform2 = (RectTransform)rectTransform2.parent;
            }
            while (canvas == null);
            switch (canvas.renderMode)
            {
                case RenderMode.ScreenSpaceOverlay:
                    return null;
                case RenderMode.ScreenSpaceCamera:
                    return (!canvas.worldCamera) ? Camera.main : canvas.worldCamera;
                default:
                    return Camera.main;
            }
        }

        public IBoxSelectable[] GetAllSelected()
        {
            if (selectables == null)
            {
                return new IBoxSelectable[0];
            }
            List<IBoxSelectable> list = new List<IBoxSelectable>();
            IBoxSelectable[] array = selectables;
            foreach (IBoxSelectable boxSelectable in array)
            {
                if (boxSelectable.selected)
                {
                    list.Add(boxSelectable);
                }
            }
            return list.ToArray();
        }

        private void EndSelection()
        {
            if (Mouse.current != null && Mouse.current.leftButton.wasReleasedThisFrame && boxRect.gameObject.activeSelf)
            {
                clickedAfterDrag = GetSelectableAtMousePosition();
                ApplySingleClickDeselection();
                ApplyPreSelections();
                ResetBoxRect();
                onSelectionChange.Invoke(GetAllSelected());
            }
        }

        private void Start()
        {
            ValidateCanvas();
            CreateBoxRect();
            ResetBoxRect();
        }

        private void Update()
        {
            BeginSelection();
            DragSelection();
            EndSelection();
        }
    }
}