using HutongGames.PlayMaker;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ClubPenguin.Adventure
{
    [ActionCategory("GUI")]
    public class GetMousePosition : FsmStateAction
    {
        public FsmVector2 MousePositionVariable;

        public override void OnEnter()
        {
            Vector2 mousePosition = Vector2.zero;
            if (Mouse.current != null)
            {
                mousePosition = Mouse.current.position.ReadValue();
            }
            MousePositionVariable.Value = mousePosition;
            Finish();
        }
    }
}