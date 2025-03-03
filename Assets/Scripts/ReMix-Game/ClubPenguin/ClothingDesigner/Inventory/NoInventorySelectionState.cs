using ClubPenguin.ClothingDesigner.ItemCustomizer;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ClubPenguin.ClothingDesigner.Inventory
{
	public class NoInventorySelectionState : DragAreaState
	{
		private CustomizerGestureModel currentGesture;

		private Camera mainCamera;

		public NoInventorySelectionState(Camera mainCamera)
		{
			this.mainCamera = mainCamera;
		}

		public override void EnterState(CustomizerGestureModel currentGesture)
		{
			this.currentGesture = currentGesture;
			InventoryContext.EventBus.DispatchEvent(default(InventoryUIEvents.EnableScrollRect));
		}

		protected override void ProcessOneTouch(ITouch touch)
		{
			switch (touch.phase)
			{
			case TouchPhase.Stationary:
				break;
			case TouchPhase.Began:
				currentGesture = processGesture(touch, currentGesture);
				break;
			case TouchPhase.Ended:
			case TouchPhase.Canceled:
				currentGesture = new CustomizerGestureModel();
				break;
			case TouchPhase.Moved:
				switch (currentGesture.TouchDownStartArea)
				{
				case AreaTouchedEnum.INVENTORY_BUTTON:
					if (checkButtonDrag(touch.deltaPosition) && currentGesture.IsEquippable)
					{
						InventoryContext.EventBus.DispatchEvent(new InventoryDragEvents.DragInventoryButton(currentGesture));
					}
					break;
				case AreaTouchedEnum.PENGUIN_PREVIEW_ROTATION_AREA:
					InventoryContext.EventBus.DispatchEvent(new InventoryDragEvents.RotatePenguinPreview(currentGesture));
					break;
				}
				break;
			}
		}

		private CustomizerGestureModel processGesture(ITouch touch, CustomizerGestureModel gestureModel)
		{
			gestureModel.TouchDownStartPos = touch.position;
			if (isOverUI(touch))
			{
				if (EventSystem.current.currentSelectedGameObject != null)
				{
					EquipmentIcon component = EventSystem.current.currentSelectedGameObject.GetComponent<EquipmentIcon>();
					if (component != null)
					{
						gestureModel.TouchDownStartArea = AreaTouchedEnum.INVENTORY_BUTTON;
						gestureModel.DragIconTexture = (component.GetIcon() as Texture2D);
						gestureModel.ItemId = component.EquipmentId;
						gestureModel.IsEquippable = component.IsEquippable;
					}
				}
			}
			else if (isTouchBlockedByUIControls(touch))
			{
				gestureModel.TouchDownStartArea = AreaTouchedEnum.CLICK_BLOCKING_UI;
			}
			else
			{
				gestureModel.TouchDownStartArea = AreaTouchedEnum.PENGUIN_PREVIEW_ROTATION_AREA;
			}
			return gestureModel;
		}

		private bool isOverUI(ITouch touch)
		{
			return touch.position.y < (float)Screen.height * mainCamera.rect.y;
		}

		private bool isTouchBlockedByUIControls(ITouch touch)
		{
			bool result = false;
			PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
			pointerEventData.position = touch.position;
			List<RaycastResult> list = new List<RaycastResult>();
			EventSystem.current.RaycastAll(pointerEventData, list);
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].gameObject.CompareTag("RaycastBlockingUI"))
				{
					result = true;
					break;
				}
			}
			return result;
		}

		public override void ExitState()
		{
			InventoryContext.EventBus.DispatchEvent(default(InventoryUIEvents.DisableScrollRect));
		}
	}
}
