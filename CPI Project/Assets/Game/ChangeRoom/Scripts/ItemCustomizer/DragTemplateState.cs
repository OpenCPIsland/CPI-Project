using Disney.LaunchPadFramework;
using UnityEngine;

namespace ClubPenguin.ClothingDesigner.ItemCustomizer
{
	internal class DragTemplateState : DragAreaState
	{
		private TemplateDefinition templateData;

		private Texture2D dragTexture;

		private DragContainer dragContainer;

		private Camera mainCamera;

		private Camera guiCamera;

		public DragTemplateState(Camera mainCamera, Camera guiCamera, DragContainer dragContainer)
		{
			this.mainCamera = mainCamera;
			this.guiCamera = guiCamera;
			this.dragContainer = dragContainer;
		}

		public override void EnterState(CustomizerGestureModel currentGesture)
		{
			templateData = currentGesture.TemplateData;
			dragTexture = currentGesture.DragIconTexture;
			if (dragTexture != null)
			{
				setupDragIcon(currentGesture);
				return;
			}
			Log.LogError(this, "Current gesture drag icon texture was null. Exiting drag template state.");
			ExitState();
			CustomizationContext.EventBus.DispatchEvent(default(CustomizerDragEvents.GestureComplete));
		}

		protected override void ProcessOneTouch(ITouch touch)
		{
			switch (touch.phase)
			{
			case TouchPhase.Stationary:
				break;
			case TouchPhase.Moved:
				onDrag(touch.position);
				break;
			case TouchPhase.Ended:
			case TouchPhase.Canceled:
				onDragEnd(touch);
				break;
			}
		}

		private void onDrag(Vector2 touchPosition)
		{
			if (!(dragContainer == null))
			{
				setRelativeIconPostion(touchPosition);
			}
		}

		private void onDragEnd(ITouch touch)
		{
			dragContainer.Hide();
			if (touch.position.y > (float)Screen.height * mainCamera.rect.y)
			{
				CustomizationContext.EventBus.DispatchEvent(new CustomizerUIEvents.SelectTemplate(templateData, dragContainer.GetSprite()));
			}
			CustomizationContext.EventBus.DispatchEvent(default(CustomizerDragEvents.GestureComplete));
		}

		public override void ExitState()
		{
		}

		protected void setupDragIcon(CustomizerGestureModel currentGesture)
		{
			dragContainer.SetImage(Sprite.Create(dragTexture, new Rect(0f, 0f, dragTexture.width, dragTexture.height), default(Vector2)));
			(dragContainer.transform as RectTransform).anchoredPosition = currentGesture.TouchDownStartPos;
			setRelativeIconPostion(currentGesture.TouchDownStartPos);
			dragContainer.Show();
		}

		protected void setRelativeIconPostion(Vector2 screenPosition)
		{
			RectTransform rectTransform = dragContainer.transform as RectTransform;
			Vector2 localPoint;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPosition, guiCamera, out localPoint);
			rectTransform.position = rectTransform.TransformPoint(localPoint);
		}
	}
}
