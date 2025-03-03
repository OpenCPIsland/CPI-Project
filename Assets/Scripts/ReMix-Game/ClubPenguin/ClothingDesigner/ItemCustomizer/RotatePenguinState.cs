using UnityEngine;

namespace ClubPenguin.ClothingDesigner.ItemCustomizer
{
	internal class RotatePenguinState : DragAreaState
	{
		private const float ROTATION_MULTIPLIER = -0.3f;

		private const float MIN_ROTATION_SPEED = -15f;

		private const float MAX_ROTATION_SPEED = 15f;

		private float previousTouchX;

		private GameObject penguinMannequin;

		private PenguinRotationAnimate _animateRotation = null;

		public RotatePenguinState(GameObject penguinMannequin)
		{
			this.penguinMannequin = penguinMannequin;
			_animateRotation = this.penguinMannequin.GetComponent<PenguinRotationAnimate>();
		}

		public override void EnterState(CustomizerGestureModel currentGesture)
		{
			previousTouchX = currentGesture.TouchDownStartPos.x;
			onStill();
		}

		public override void ExitState()
		{
		}

		protected override void ProcessOneTouch(ITouch touch)
		{
			switch (touch.phase)
			{
			case TouchPhase.Ended:
			case TouchPhase.Canceled:
				onDrag(touch.position, true);
				CustomizationContext.EventBus.DispatchEvent(default(CustomizerDragEvents.GestureComplete));
				break;
			case TouchPhase.Moved:
				onDrag(touch.position, false);
				break;
			default:
				onStill();
				break;
			}
		}

		private void onDrag(Vector2 touchPosition, bool isRelease)
		{
			float num = touchPosition.x - previousTouchX;
			float num2 = Mathf.Clamp(-0.3f * num, -15f, 15f);
			if (_animateRotation != null)
			{
				if (isRelease)
				{
					_animateRotation.AddReleaseForce();
				}
				else
				{
					_animateRotation.OffsetRotation(num2);
				}
			}
			else
			{
				penguinMannequin.transform.Rotate(new Vector3(0f, num2, 0f));
			}
			previousTouchX = touchPosition.x;
		}

		private void onStill()
		{
			if (_animateRotation != null)
			{
				_animateRotation.OffsetRotation(0f);
			}
		}
	}
}
