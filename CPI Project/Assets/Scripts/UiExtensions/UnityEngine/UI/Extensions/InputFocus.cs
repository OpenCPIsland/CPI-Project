using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

namespace UnityEngine.UI.Extensions
{
    [RequireComponent(typeof(InputField))]
    [AddComponentMenu("UI/Extensions/InputFocus")]
    public class InputFocus : MonoBehaviour
    {
        protected InputField _inputField;

        public bool _ignoreNextActivation = false;

        private void Start()
        {
            _inputField = GetComponent<InputField>();
        }

        private void Update()
        {
            // Use new Input System for Return key detection
            if (Keyboard.current != null && Keyboard.current[Key.Enter].wasReleasedThisFrame && !_inputField.isFocused)
            {
                if (_ignoreNextActivation)
                {
                    _ignoreNextActivation = false;
                    return;
                }
                _inputField.Select();
                _inputField.ActivateInputField();
            }
        }

        public void buttonPressed()
        {
            bool flag = _inputField.text == "";
            _inputField.text = "";
            if (!flag)
            {
                _inputField.Select();
                _inputField.ActivateInputField();
            }
        }

        public void OnEndEdit(string textString)
        {
            // Use new Input System for Return key detection
            if (Keyboard.current != null && Keyboard.current[Key.Enter].wasPressedThisFrame)
            {
                bool flag = _inputField.text == "";
                _inputField.text = "";
                if (flag)
                {
                    _ignoreNextActivation = true;
                }
            }
        }
    }
}