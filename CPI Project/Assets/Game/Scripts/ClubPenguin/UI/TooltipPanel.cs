using UnityEngine;
using UnityEngine.InputSystem;

namespace ClubPenguin.UI
{
    public class TooltipPanel : MonoBehaviour
    {
        private bool isShowing;

        private void Update()
        {
            // Use new Input System to detect a left mouse button press
            if (isShowing && Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            {
                isShowing = false;
                base.gameObject.SetActive(isShowing);
            }
        }

        public void ShowTooltip()
        {
            if (!isShowing)
            {
                isShowing = true;
                base.gameObject.SetActive(isShowing);
            }
        }
    }
}