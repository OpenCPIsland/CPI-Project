using System;
using System.Collections.Generic;
using System.Linq;
using Tweaker.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Tweaker.UI
{
    public class KeyBindingManager : MonoBehaviour
    {
        private ITweakerLogger logger = LogManager.GetCurrentClassLogger();
        private readonly Dictionary<Key, IInvokable> bindings = new Dictionary<Key, IInvokable>();
        private readonly Dictionary<Key, InputAction> actions = new Dictionary<Key, InputAction>();

        private Key currentPress = Key.None;
        private float secondsUntilActivation;

        public void Init(IEnumerable<IInvokable> invokables)
        {
            foreach (IInvokable invokable in invokables)
            {
                KeyBindingAttribute keyBindingAttribute = invokable.CustomAttributes.FirstOrDefault((ICustomTweakerAttribute a) => a is KeyBindingAttribute) as KeyBindingAttribute;
                if (keyBindingAttribute != null)
                {
                    if (invokable.Parameters.Length != 0)
                    {
                        throw new InvalidOperationException("Cannot add key binding to Invokable that requires arguments.");
                    }
                    logger.Debug("Found Key Binding: {0} => {1}", keyBindingAttribute.Key, invokable.Name);

                    Key inputSystemKey = ConvertToInputSystemKey(keyBindingAttribute.Key);
                    bindings.Add(inputSystemKey, invokable);

                    var inputAction = new InputAction(type: InputActionType.Button, binding: $"<Keyboard>/{inputSystemKey.ToString().ToLower()}");
                    inputAction.performed += ctx => OnKeyPress(inputSystemKey);
                    inputAction.canceled += ctx => OnKeyRelease(inputSystemKey);
                    inputAction.Enable();
                    actions.Add(inputSystemKey, inputAction);
                }
            }
        }

        private void OnKeyPress(Key key)
        {
            if (currentPress == Key.None)
            {
                currentPress = key;
                secondsUntilActivation = 1f;
            }
        }

        private void OnKeyRelease(Key key)
        {
            if (currentPress == key)
            {
                currentPress = Key.None;
            }
        }

        private void Update()
        {
            if (currentPress != Key.None)
            {
                secondsUntilActivation -= Time.unscaledDeltaTime;
                if (secondsUntilActivation <= 0f)
                {
                    var key = currentPress;
                    currentPress = Key.None;
                    if (bindings.TryGetValue(key, out var invokable))
                    {
                        invokable.Invoke(null);
                    }
                }
            }
        }

        private void OnDisable()
        {
            foreach (var action in actions.Values)
            {
                action.Disable();
                action.Dispose();
            }
            actions.Clear();
        }

        // Utility: Convert legacy KeyCode to new InputSystem Key
        private Key ConvertToInputSystemKey(KeyCode keyCode)
        {
            // This covers common alphanumerics and function keys; expand as needed.
            // You may want to use a more comprehensive mapping for your full key set.

            if (Enum.TryParse<Key>(keyCode.ToString(), out var key))
            {
                return key;
            }
            throw new ArgumentException($"Cannot convert KeyCode {keyCode} to InputSystem Key.");
        }
    }
}