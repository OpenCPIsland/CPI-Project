// (c) Copyright HutongGames. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GUI)]
	[Tooltip("Controls the appearance of Mouse Cursor.")]
	public class SetMouseCursor : FsmStateAction
	{
        [Tooltip("The texture to use for the cursor.")]
		public FsmTexture cursorTexture;

        [Tooltip("Hide the cursor.")]
		public FsmBool hideCursor;

        [Tooltip("Lock the cursor to the center of the screen. Useful in first person controllers.")]
		public FsmBool lockCursor;

        [Tooltip("Repeat every frame.")]
        public bool everyFrame;

		public override void Reset()
		{
			cursorTexture = null;
			hideCursor = false;
			lockCursor = false;
            everyFrame = false;
        }
		
		public override void OnEnter()
		{
			PlayMakerGUI.LockCursor = lockCursor.Value;
			PlayMakerGUI.HideCursor = hideCursor.Value;
			PlayMakerGUI.MouseCursor = cursorTexture.Value;

            // Don't rely on PlayMakerGUI to update the cursor since it may not be in the scene.
            // If it is in the scene, this is redundant, but that's fine...

            UpdateCursorState();

            if (!everyFrame)
            {
                Finish();
            }
		}

        private void UpdateCursorState()
        {
            Cursor.lockState = lockCursor.Value ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !hideCursor.Value;
        }

		public override void OnUpdate()
		{
			UpdateCursorState();
		}
		
		public override void OnGUI()
        {
            // Handled by PlayMakerGUI?
            
            if (PlayMakerGUI.Exists) return;

			// draw custom cursor

            var texture = cursorTexture.Value;

			if (texture != null)
            {
                var mousePos = ActionHelpers.GetMousePosition();
				var pos =  new Rect(mousePos.x - texture.width * 0.5f, 
					Screen.height - mousePos.y - texture.height * 0.5f, 
                    texture.width, texture.height);
				
				GUI.DrawTexture(pos, texture);
			}
		}
	}
}
