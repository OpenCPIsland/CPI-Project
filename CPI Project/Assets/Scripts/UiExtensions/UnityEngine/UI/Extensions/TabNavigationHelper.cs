using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem; // New Input System
using Key = UnityEngine.InputSystem.Key; // Alias for clarity

namespace UnityEngine.UI.Extensions
{
    [RequireComponent(typeof(EventSystem))]
    [AddComponentMenu("Event/Extensions/Tab Navigation Helper")]
    public class TabNavigationHelper : MonoBehaviour
    {
        private EventSystem _system;

        [Tooltip("The path to take when user is tabbing through ui components.")]
        public Selectable[] NavigationPath;

        [Tooltip("Use the default Unity navigation system or a manual fixed order using Navigation Path")]
        public NavigationMode NavigationMode;

        private void Start()
        {
            _system = GetComponent<EventSystem>();
            if (_system == null)
            {
                Debug.LogError("Needs to be attached to the Event System component in the scene");
            }
        }

        public void Update()
        {
            Selectable selectable = null;
            bool tabDown = Keyboard.current != null && Keyboard.current[Key.Tab].wasPressedThisFrame;
            bool shiftHeld = Keyboard.current != null && (Keyboard.current[Key.LeftShift].isPressed || Keyboard.current[Key.RightShift].isPressed);

            if (tabDown && shiftHeld)
            {
                selectable = (_system.currentSelectedGameObject != null)
                    ? _system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnUp()
                    : _system.firstSelectedGameObject.GetComponent<Selectable>();
            }
            else if (tabDown)
            {
                selectable = (_system.currentSelectedGameObject != null)
                    ? _system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown()
                    : _system.firstSelectedGameObject.GetComponent<Selectable>();
            }
            else if (NavigationMode == NavigationMode.Manual && _system.currentSelectedGameObject != null)
            {
                for (int i = 0; i < NavigationPath.Length; i++)
                {
                    if (_system.currentSelectedGameObject == NavigationPath[i].gameObject)
                    {
                        selectable = (i != NavigationPath.Length - 1) ? NavigationPath[i + 1] : NavigationPath[0];
                        break;
                    }
                }
            }
            else if (_system.currentSelectedGameObject == null)
            {
                selectable = _system.firstSelectedGameObject.GetComponent<Selectable>();
            }
            selectGameObject(selectable);
        }

        private void selectGameObject(Selectable selectable)
        {
            if (selectable != null)
            {
                InputField component = selectable.GetComponent<InputField>();
                if (component != null)
                {
                    component.OnPointerClick(new PointerEventData(_system));
                }
                _system.SetSelectedGameObject(selectable.gameObject, new BaseEventData(_system));
            }
        }
    }
}