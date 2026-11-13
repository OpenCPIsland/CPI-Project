using System;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace UnityEngine.UI.Extensions
{
    [RequireComponent(typeof(InputField))]
    [AddComponentMenu("UI/Extensions/Input Field Submit")]
    public class InputFieldEnterSubmit : MonoBehaviour
    {
        [Serializable]
        public class EnterSubmitEvent : UnityEvent<string>
        {
        }

        public EnterSubmitEvent EnterSubmit;

        private InputField _input;

        private void Awake()
        {
            _input = GetComponent<InputField>();
            _input.onEndEdit.AddListener(OnEndEdit);
        }

        public void OnEndEdit(string txt)
        {
            // Use new Input System for Enter detection
            if ((Keyboard.current != null && Keyboard.current[Key.Enter].wasPressedThisFrame) ||
                (Keyboard.current != null && Keyboard.current[Key.NumpadEnter].wasPressedThisFrame))
            {
                EnterSubmit.Invoke(txt);
            }
        }
    }
}