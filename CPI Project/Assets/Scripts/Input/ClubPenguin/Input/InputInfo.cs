using DevonLocalization.Core;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace ClubPenguin.Input
{
    public abstract class InputInfo
    {
        public abstract void Populate(ControlScheme controlScheme);

        // Overload for new Input System Key
        protected string getKeyTranslation(Key key)
        {
            if (key == Key.None)
            {
                return string.Empty;
            }
            string keyStr = key.ToString();
            string locKey = $"Input.KeyCodes.{keyStr}";
            string value;
            return Service.Get<Localizer>().tokens.TryGetValue(locKey, out value) ? value : keyStr;
        }

        // Legacy support for KeyCode
        protected string getKeyCodeTranslation(KeyCode keyCode)
        {
            if (keyCode == KeyCode.None)
            {
                return string.Empty;
            }
            string key = $"Input.KeyCodes.{keyCode}";
            string value;
            return Service.Get<Localizer>().tokens.TryGetValue(key, out value) ? value : keyCode.ToString();
        }
    }
}