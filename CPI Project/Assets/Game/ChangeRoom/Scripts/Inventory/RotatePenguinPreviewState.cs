using UnityEngine;

namespace ClubPenguin.ClothingDesigner.Inventory
{
	public class RotatePenguinPreviewState : DragAreaState
	{
		private const float ROTATION_MULTIPLIER = -0.3f;

		private const float MIN_ROTATION_SPEED = -15f;

		private const float MAX_ROTATION_SPEED = 15f;

		private readonly Transform penguinPreview;

		private float previousTouchX;

		public RotatePenguinPreviewState(Transform penguinPreview)
		{
			this.penguinPreview = penguinPreview;
		}

		public override void EnterState(CustomizerGestureModel currentGesture)
		{
			previousTouchX = currentGesture.TouchDownStartPos.x;
		}

		protected override void ProcessOneTouch(ITouch touch)
		{
			switch (touch.phase)
			{
			case TouchPhase.Stationary:
				break;
			case TouchPhase.Ended:
			case TouchPhase.Canceled:
				InventoryContext.EventBus.DispatchEvent(default(InventoryDragEvents.GestureComplete));
				break;
			case TouchPhase.Moved:
				onDrag(touch.position);
				break;
			}
		}

		private void onDrag(Vector2 touchPosition)
		{
			float num = touchPosition.x - previousTouchX;
			float y = Mathf.Clamp(-0.3f * num, -15f, 15f);
			penguinPreview.Rotate(new Vector3(0f, y, 0f));
			previousTouchX = touchPosition.x;
		}

		public override void ExitState()
		{
		}
	}
}
