using UnityEngine;
using UnityEngine.InputSystem;

namespace ClubPenguin.Input
{
    public class AnyKeyInputMap : InputMap<AnyKeyInputMap.Result>
    {
        public class Result
        {
            public readonly ButtonInputResult AnyKey = new ButtonInputResult();
        }

        public override void AddHandler(InputHandlerCallback<Result> handler)
        {
            mapResult.AnyKey.Reset();
            base.AddHandler(handler);
        }

        protected override bool processInput(ControlScheme controlScheme)
        {
            bool anyKey = false;

            if (Keyboard.current != null && Keyboard.current.allKeys.Count > 0)
            {
                foreach (var keyControl in Keyboard.current.allKeys)
                {
                    if (keyControl != null && keyControl.isPressed)
                    {
                        anyKey = true;
                        break;
                    }
                }
            }

            if (Mouse.current != null)
            {
                if ((Mouse.current.leftButton != null && Mouse.current.leftButton.isPressed) ||
                    (Mouse.current.rightButton != null && Mouse.current.rightButton.isPressed) ||
                    (Mouse.current.middleButton != null && Mouse.current.middleButton.isPressed) ||
                    (Mouse.current.forwardButton != null && Mouse.current.forwardButton.isPressed) ||
                    (Mouse.current.backButton != null && Mouse.current.backButton.isPressed))
                {
                    anyKey = true;
                }
            }

            mapResult.AnyKey.WasJustPressed = (anyKey && !mapResult.AnyKey.IsHeld);
            mapResult.AnyKey.WasJustReleased = (!anyKey && mapResult.AnyKey.IsHeld);
            mapResult.AnyKey.IsHeld = anyKey;

            return mapResult.AnyKey.IsHeld || mapResult.AnyKey.WasJustReleased;
        }
    }
}
