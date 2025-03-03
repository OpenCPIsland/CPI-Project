using ClubPenguin.Breadcrumbs;
using ClubPenguin.UI;
using CpRemixShaders;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections.Generic;
using Tweaker.Core;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ClubPenguin.ClothingDesigner.ItemCustomizer
{
	public class NoSelectionState : DragAreaState
	{
		private static float tapTimeThreshold = 0.5f;

		private static float tapDistanceThreshold = Screen.dpi / 4f;

		private Camera mainCam;

		private Vector2 dragIconStartingPos;

		private CustomizerGestureModel currentGesture;

		private OriginalFabricData originalFabricChannelData;

		private OriginalDecalData originalDecalChannelData;

		private NotificationBreadcrumbController breadcrumbController;

		private PersistentBreadcrumbTypeDefinitionKey templateBreadcrumbType;

		private PersistentBreadcrumbTypeDefinitionKey fabricBreadcrumbType;

		private PersistentBreadcrumbTypeDefinitionKey decalBreadcrumbType;

		[Tweakable("UI.TapTimeThreshold", Description = "Time allowed for registering a button tap verses.")]
		private static float TapTimeThreshold
		{
			get
			{
				return tapTimeThreshold;
			}
			set
			{
				tapTimeThreshold = value;
			}
		}

		public CustomizerState CurrentState
		{
			get;
			set;
		}

		public NoSelectionState(Camera mainCam, OriginalFabricData originalFabricChannelData, OriginalDecalData originalDecalChannelData, PersistentBreadcrumbTypeDefinitionKey templateBreadcrumbType, PersistentBreadcrumbTypeDefinitionKey fabricBreadcrumbType, PersistentBreadcrumbTypeDefinitionKey decalBreadcrumbType)
		{
			this.mainCam = mainCam;
			this.originalFabricChannelData = originalFabricChannelData;
			this.originalDecalChannelData = originalDecalChannelData;
			this.templateBreadcrumbType = templateBreadcrumbType;
			this.fabricBreadcrumbType = fabricBreadcrumbType;
			this.decalBreadcrumbType = decalBreadcrumbType;
			breadcrumbController = Service.Get<NotificationBreadcrumbController>();
		}

		public override void EnterState(CustomizerGestureModel currentGesture)
		{
			this.currentGesture = currentGesture;
			CustomizationContext.EventBus.DispatchEvent(default(CustomizerUIEvents.EnableScrollRect));
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
				if (Time.time - currentGesture.TouchStartTime < TapTimeThreshold && Vector2.Distance(touch.position, currentGesture.TouchDownStartPos) < tapDistanceThreshold)
				{
					switch (currentGesture.TouchDownStartArea)
					{
					case AreaTouchedEnum.GREEN_CHANNEL:
						if (CurrentState == CustomizerState.FABRIC)
						{
							CustomizationContext.EventBus.DispatchEvent(new CustomizerUIEvents.UpdateChannelFabric(CustomizationChannel.GREEN, originalFabricChannelData.ActualChannelGreen, originalFabricChannelData.UVOffsetGreen));
						}
						else
						{
							CustomizationContext.EventBus.DispatchEvent(new CustomizerUIEvents.UpdateChannelDecal(CustomizationChannel.GREEN, originalDecalChannelData.ActualChannelGreen, originalDecalChannelData.ActualGreenUVOffset, originalDecalChannelData.ActualGreenRenderer));
						}
						CustomizationContext.EventBus.DispatchEvent(new CustomizerUIEvents.InputOverChannel(CustomizationChannel.GREEN));
						break;
					case AreaTouchedEnum.BLUE_CHANNEL:
						if (CurrentState == CustomizerState.FABRIC)
						{
							CustomizationContext.EventBus.DispatchEvent(new CustomizerUIEvents.UpdateChannelFabric(CustomizationChannel.BLUE, originalFabricChannelData.ActualChannelBlue, originalFabricChannelData.UVOffsetBlue));
						}
						else
						{
							CustomizationContext.EventBus.DispatchEvent(new CustomizerUIEvents.UpdateChannelDecal(CustomizationChannel.BLUE, originalDecalChannelData.ActualChannelBlue, originalDecalChannelData.ActualBlueUVOffset, originalDecalChannelData.ActualBlueRenderer));
						}
						CustomizationContext.EventBus.DispatchEvent(new CustomizerUIEvents.InputOverChannel(CustomizationChannel.BLUE));
						break;
					case AreaTouchedEnum.RED_CHANNEL:
						if (CurrentState == CustomizerState.FABRIC)
						{
							CustomizationContext.EventBus.DispatchEvent(new CustomizerUIEvents.UpdateChannelFabric(CustomizationChannel.RED, originalFabricChannelData.ActualChannelRed, originalFabricChannelData.UVOffsetRed));
						}
						else
						{
							CustomizationContext.EventBus.DispatchEvent(new CustomizerUIEvents.UpdateChannelDecal(CustomizationChannel.RED, originalDecalChannelData.ActualChannelRed, originalDecalChannelData.ActualRedUVOffset, originalDecalChannelData.ActualRedRenderer));
						}
						CustomizationContext.EventBus.DispatchEvent(new CustomizerUIEvents.InputOverChannel(CustomizationChannel.RED));
						break;
					case AreaTouchedEnum.TEMPLATE_BUTTON:
						if (currentGesture.IsEnabled)
						{
							if (currentGesture.IsEquippable)
							{
								Sprite templateSprite = Sprite.Create(currentGesture.DragIconTexture, new Rect(0f, 0f, currentGesture.DragIconTexture.width, currentGesture.DragIconTexture.height), Vector2.zero);
								breadcrumbController.RemovePersistentBreadcrumb(templateBreadcrumbType, currentGesture.ItemDefinitionId.ToString());
								CustomizationContext.EventBus.DispatchEvent(new CustomizerUIEvents.SelectTemplate(currentGesture.TemplateData, templateSprite));
							}
							else
							{
								ClothingDesignerContext.EventBus.DispatchEvent(new ClothingDesignerUIEvents.ShowMemberFlow("blueprints"));
							}
						}
						break;
					}
				}
				currentGesture = new CustomizerGestureModel();
				break;
			case TouchPhase.Canceled:
				currentGesture = new CustomizerGestureModel();
				break;
			case TouchPhase.Moved:
				switch (currentGesture.TouchDownStartArea)
				{
				case AreaTouchedEnum.CLICK_BLOCKING_UI:
					break;
				case AreaTouchedEnum.RED_CHANNEL:
				case AreaTouchedEnum.GREEN_CHANNEL:
				case AreaTouchedEnum.BLUE_CHANNEL:
					CustomizationContext.EventBus.DispatchEvent(new CustomizerDragEvents.DragOffChannel(currentGesture));
					break;
				case AreaTouchedEnum.FABRIC_BUTTON:
					if (checkButtonDrag(touch.deltaPosition))
					{
						breadcrumbController.RemovePersistentBreadcrumb(fabricBreadcrumbType, currentGesture.ItemDefinitionId.ToString());
						CustomizationContext.EventBus.DispatchEvent(new CustomizerDragEvents.DragFabricButton(currentGesture));
					}
					break;
				case AreaTouchedEnum.DECAL_BUTTON:
					if (checkButtonDrag(touch.deltaPosition))
					{
						breadcrumbController.RemovePersistentBreadcrumb(decalBreadcrumbType, currentGesture.ItemDefinitionId.ToString());
						CustomizationContext.EventBus.DispatchEvent(new CustomizerDragEvents.DragDecalButton(currentGesture));
					}
					break;
				case AreaTouchedEnum.TEMPLATE_BUTTON:
					if (checkButtonDrag(touch.deltaPosition) && currentGesture.IsEquippable)
					{
						breadcrumbController.RemovePersistentBreadcrumb(templateBreadcrumbType, currentGesture.ItemDefinitionId.ToString());
						CustomizationContext.EventBus.DispatchEvent(new CustomizerDragEvents.DragTemplate(currentGesture));
					}
					break;
				case AreaTouchedEnum.PENGUIN_ROTATION_AREA:
					CustomizationContext.EventBus.DispatchEvent(new CustomizerDragEvents.RotatePenguin(currentGesture));
					break;
				}
				break;
			}
		}

		private CustomizerGestureModel processGesture(ITouch touch, CustomizerGestureModel gestureModel)
		{
			gestureModel.TouchDownStartPos = touch.position;
			gestureModel.TouchStartTime = Time.time;
			if (isOverUI(touch))
			{
				PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
				pointerEventData.position = touch.position;
				List<RaycastResult> list = new List<RaycastResult>();
				EventSystem.current.RaycastAll(pointerEventData, list);
				GameObject gameObject = null;
				float num = 0f;
				if (list != null && list.Count > 0)
				{
					for (int i = 0; i < list.Count; i++)
					{
						if (num <= list[i].index)
						{
							gameObject = list[i].gameObject;
							num = list[i].index;
						}
					}
				}
				if (gameObject != null)
				{
					CustomizationButton component = gameObject.GetComponent<CustomizationButton>();
					if (component != null && component.CanDrag && component.GetTexture != null)
					{
						switch (component.DraggableButtonType)
						{
						case DraggableButtonType.TEMPLATE:
						{
							TemplateIcon templateIcon = component as TemplateIcon;
							gestureModel.TouchDownStartArea = AreaTouchedEnum.TEMPLATE_BUTTON;
							gestureModel.DragIconTexture = component.GetTexture;
							gestureModel.TemplateData = templateIcon.TemplateData;
							gestureModel.IsEquippable = templateIcon.CanSelect;
							gestureModel.ItemDefinitionId = templateIcon.DefinitionId;
							gestureModel.IsEnabled = templateIcon.IsEnabled;
							break;
						}
						case DraggableButtonType.FABRIC:
							gestureModel.TouchDownStartArea = AreaTouchedEnum.FABRIC_BUTTON;
							gestureModel.DragIconTexture = component.GetTexture;
							gestureModel.ItemDefinitionId = component.DefinitionId;
							break;
						case DraggableButtonType.DECAL:
							gestureModel.TouchDownStartArea = AreaTouchedEnum.DECAL_BUTTON;
							gestureModel.DragIconTexture = component.GetTexture;
							gestureModel.ItemDefinitionId = component.DefinitionId;
							break;
						}
					}
				}
			}
			else if (isTouchBlockedByUIControls(touch))
			{
				gestureModel.TouchDownStartArea = AreaTouchedEnum.CLICK_BLOCKING_UI;
			}
			else
			{
				Ray ray = Camera.main.ScreenPointToRay(touch.position);
				RaycastHit hitInfo;
				if (Physics.Raycast(ray, out hitInfo))
				{
					GameObject gameObject2 = hitInfo.collider.gameObject;
					if (gameObject2.GetComponent<SkinnedMeshRenderer>() != null && gameObject2.GetComponent<SkinnedMeshRenderer>().sharedMaterial.shader.name.Contains("Equipment"))
					{
						Texture2D texture2D = gameObject2.GetComponent<SkinnedMeshRenderer>().sharedMaterial.GetTexture(EquipmentShaderParams.DECALS_123_OPACITY_TEX) as Texture2D;
						if (texture2D == null)
						{
							Log.LogErrorFormatted(this, "Unable to retrieve decal 123 map on item {0}. Property did not exist {1}.", gameObject2.name, EquipmentShaderParams.DECALS_123_OPACITY_TEX);
							return gestureModel;
						}
						Vector2 textureCoord = hitInfo.textureCoord;
						textureCoord.Scale(new Vector2(texture2D.width, texture2D.height));
						Color pixel = texture2D.GetPixel((int)textureCoord.x, (int)textureCoord.y);
						if (Mathf.Abs(pixel.r) < Mathf.Epsilon && Mathf.Abs(pixel.g) < Mathf.Epsilon && Mathf.Abs(pixel.b) < Mathf.Epsilon)
						{
							gestureModel.TouchDownStartArea = AreaTouchedEnum.NOTHING;
						}
						else if (pixel.r > pixel.g && pixel.r > pixel.b)
						{
							gestureModel.TouchDownStartArea = AreaTouchedEnum.RED_CHANNEL;
							gestureModel.StartGameObject = gameObject2;
						}
						else if (pixel.g > pixel.r && pixel.g > pixel.b)
						{
							gestureModel.TouchDownStartArea = AreaTouchedEnum.GREEN_CHANNEL;
							gestureModel.StartGameObject = gameObject2;
						}
						else
						{
							gestureModel.TouchDownStartArea = AreaTouchedEnum.BLUE_CHANNEL;
							gestureModel.StartGameObject = gameObject2;
						}
					}
					else
					{
						gestureModel.TouchDownStartArea = AreaTouchedEnum.PENGUIN_ROTATION_AREA;
					}
				}
				else
				{
					gestureModel.TouchDownStartArea = AreaTouchedEnum.PENGUIN_ROTATION_AREA;
				}
			}
			return gestureModel;
		}

		private bool isOverUI(ITouch touch)
		{
			return touch.position.y < (float)Screen.height * mainCam.rect.y;
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
			CustomizationContext.EventBus.DispatchEvent(default(CustomizerUIEvents.DisableScrollRect));
		}
	}
}
