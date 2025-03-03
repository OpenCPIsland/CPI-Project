using UnityEngine;

namespace ClubPenguin.ClothingDesigner.Inventory
{
	public class DragInventoryButtonState : DragAreaState
	{
		protected Camera mainCamera;

		private DragContainer dragContainer;

		private Camera guiCamera;

		private long equipmentId;

		public DragInventoryButtonState(DragContainer dragContainer, Camera mainCamera, Camera guiCamera)
		{
			this.dragContainer = dragContainer;
			this.mainCamera = mainCamera;
			this.guiCamera = guiCamera;
		}

		public override void EnterState(CustomizerGestureModel currentGesture)
		{
			equipmentId = currentGesture.ItemId;
			setupDragIcon(currentGesture);
		}

		protected void setupDragIcon(CustomizerGestureModel currentGesture)
		{
			dragContainer.SetImage(Sprite.Create(currentGesture.DragIconTexture, new Rect(0f, 0f, currentGesture.DragIconTexture.width, currentGesture.DragIconTexture.height), default(Vector2)));
			(dragContainer.transform as RectTransform).anchoredPosition = currentGesture.TouchDownStartPos;
			setRelativeIconPostion(currentGesture.TouchDownStartPos);
			dragContainer.Show();
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
				onDragEnd(touch.position);
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

		protected virtual void onDragEnd(Vector2 touchPosition)
		{
			Ray ray = mainCamera.ScreenPointToRay(touchPosition);
			RaycastHit hitInfo;
			if (Physics.Raycast(ray, out hitInfo))
			{
				GameObject gameObject = hitInfo.collider.gameObject;
				if (gameObject.GetComponent<InventoryAvatarController>() != null)
				{
					InventoryContext.EventBus.DispatchEvent(new InventoryUIEvents.EquipEquipment(equipmentId));
				}
			}
			InventoryContext.EventBus.DispatchEvent(default(InventoryDragEvents.GestureComplete));
		}

		private void setRelativeIconPostion(Vector2 screenPosition)
		{
			RectTransform rectTransform = dragContainer.transform as RectTransform;
			Vector2 localPoint;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPosition, guiCamera, out localPoint);
			rectTransform.position = rectTransform.TransformPoint(localPoint);
		}

		public override void ExitState()
		{
			if (dragContainer != null)
			{
				dragContainer.Hide();
			}
		}
	}
}
