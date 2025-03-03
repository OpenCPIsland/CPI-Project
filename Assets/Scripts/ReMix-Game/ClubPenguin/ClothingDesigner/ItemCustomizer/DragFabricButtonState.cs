using ClubPenguin.Core;
using ClubPenguin.UI;
using CpRemixShaders;
using Disney.LaunchPadFramework;
using Fabric;
using UnityEngine;

namespace ClubPenguin.ClothingDesigner.ItemCustomizer
{
	internal class DragFabricButtonState : DragAreaState
	{
		protected GameObject dragArea;

		protected DragContainer dragContainer;

		protected Texture2D dragTexture;

		protected Camera guiCamera;

		protected Camera mainCamera;

		protected Vector3 lastTouchedPosition;

		protected CustomizationChannel lastChannel = CustomizationChannel.NONE;

		protected Vector2 lastUVPosition = Vector2.zero;

		protected CustomizationChannel channelUpdated;

		protected OriginalFabricData fabricChannelData;

		public Vector2 DragIconOffset
		{
			get;
			set;
		}

		public DragFabricButtonState(GameObject dragArea, Camera mainCamera, Camera guiCamera, DragContainer dragContainer)
		{
			this.dragArea = dragArea;
			this.mainCamera = mainCamera;
			this.guiCamera = guiCamera;
			this.dragContainer = dragContainer;
		}

		public void SetPersistentChannelData(OriginalFabricData fabricChannelData)
		{
			this.fabricChannelData = fabricChannelData;
			channelUpdated = CustomizationChannel.NONE;
		}

		public override void EnterState(CustomizerGestureModel currentGesture)
		{
			fabricChannelData.OriginalChannelRed = fabricChannelData.ActualChannelRed;
			fabricChannelData.OriginalChannelGreen = fabricChannelData.ActualChannelGreen;
			fabricChannelData.OriginalChannelBlue = fabricChannelData.ActualChannelBlue;
			channelUpdated = CustomizationChannel.NONE;
			dragTexture = currentGesture.DragIconTexture;
			if (dragTexture != null)
			{
				CustomizationContext.EventBus.DispatchEvent(new CustomizerDragEvents.StartDragFabricButton(currentGesture));
				CustomizationContext.EventBus.DispatchEvent(new CustomizerUIEvents.InputStateChange(true, false));
				CustomizationContext.EventBus.DispatchEvent(new CustomizerUIEvents.InputOverChannel(CustomizationChannel.NONE));
				setupDragIcon(currentGesture);
				EventManager.Instance.PostEvent("SFX/UI/ClothingDesigner/ItemSelect", EventAction.PlaySound);
			}
			else
			{
				CustomizationContext.EventBus.DispatchEvent(default(CustomizerDragEvents.EndDragFabricButton));
				CustomizationContext.EventBus.DispatchEvent(default(CustomizerDragEvents.GestureComplete));
				CustomizationContext.EventBus.DispatchEvent(new CustomizerUIEvents.InputStateChange(false, false));
			}
		}

		protected void setupDragIcon(CustomizerGestureModel currentGesture)
		{
			dragContainer.SetImage(Sprite.Create(dragTexture, new Rect(0f, 0f, dragTexture.width, dragTexture.height), default(Vector2)));
			(dragContainer.transform as RectTransform).anchoredPosition = currentGesture.TouchDownStartPos;
			setRelativeIconPostion(currentGesture.TouchDownStartPos);
		}

		public override void ExitState()
		{
			lastChannel = CustomizationChannel.NONE;
			if (dragContainer != null)
			{
				dragContainer.Hide();
			}
		}

		protected override void ProcessOneTouch(ITouch touch)
		{
			switch (touch.phase)
			{
			case TouchPhase.Stationary:
				break;
			case TouchPhase.Ended:
			case TouchPhase.Canceled:
				onDragEnd();
				break;
			case TouchPhase.Moved:
				onDrag(touch.position);
				break;
			}
		}

		protected void onDrag(Vector2 touchPosition)
		{
			if (dragContainer == null)
			{
				return;
			}
			bool flag = true;
			float num = 1136f;
			GameObject gameObject = GameObject.FindWithTag("UIHud");
			if (gameObject != null)
			{
				num = gameObject.GetComponentInChildren<CanvasScalerExt>().ReferenceResolutionY;
			}
			bool flag2 = PlatformUtils.GetPlatformType() == PlatformType.Standalone;
			Vector2 v = touchPosition;
			if (!flag2)
			{
				v += DragIconOffset * ((float)Screen.height / num);
			}
			Ray ray = mainCamera.ScreenPointToRay(v);
			RaycastHit hitInfo;
			if (Physics.Raycast(ray, out hitInfo))
			{
				GameObject gameObject2 = hitInfo.collider.gameObject;
				SkinnedMeshRenderer component = gameObject2.GetComponent<SkinnedMeshRenderer>();
				if (component != null && component.sharedMaterial.shader.name.Contains("Equipment"))
				{
					lastTouchedPosition = touchPosition;
					Texture2D texture2D = gameObject2.GetComponent<SkinnedMeshRenderer>().sharedMaterial.GetTexture(EquipmentShaderParams.DECALS_123_OPACITY_TEX) as Texture2D;
					if (texture2D == null)
					{
						Log.LogErrorFormatted(this, "Unable to retrieve decal 123 map on item {0}. Property did not exist {1}.", gameObject2.name, EquipmentShaderParams.DECALS_123_OPACITY_TEX);
						return;
					}
					Vector2 textureCoord = hitInfo.textureCoord;
					textureCoord.Scale(new Vector2(texture2D.width, texture2D.height));
					Color pixel = texture2D.GetPixel((int)textureCoord.x, (int)textureCoord.y);
					if (Mathf.Abs(pixel.r) < Mathf.Epsilon && Mathf.Abs(pixel.g) < Mathf.Epsilon && Mathf.Abs(pixel.b) < Mathf.Epsilon)
					{
						flag = true;
					}
					else if (pixel.r > pixel.g && pixel.r > pixel.b)
					{
						dragChannelRed(hitInfo.textureCoord, component);
						flag = false;
					}
					else if (pixel.g > pixel.r && pixel.g > pixel.b)
					{
						dragChannelGreen(hitInfo.textureCoord, component);
						flag = false;
					}
					else
					{
						dragChannelBlue(hitInfo.textureCoord, component);
						flag = false;
					}
				}
			}
			if (flag)
			{
				dragContainer.Show();
				setRelativeIconPostion(touchPosition);
				if (channelUpdated != 0)
				{
					dragChannelNone();
				}
			}
			else
			{
				dragContainer.Hide(true);
			}
		}

		protected virtual void dragChannelRed(Vector2 textureCoord, SkinnedMeshRenderer smr)
		{
			Vector2 vector = DragChannel(CustomizationChannel.RED, textureCoord, fabricChannelData.UVOffsetRed);
			fabricChannelData.UVOffsetRed = vector;
			CustomizationContext.EventBus.DispatchEvent(new CustomizerUIEvents.UpdateChannelFabric(CustomizationChannel.GREEN, fabricChannelData.OriginalChannelGreen, fabricChannelData.UVOffsetGreen));
			CustomizationContext.EventBus.DispatchEvent(new CustomizerUIEvents.UpdateChannelFabric(CustomizationChannel.BLUE, fabricChannelData.OriginalChannelBlue, fabricChannelData.UVOffsetBlue));
			CustomizationContext.EventBus.DispatchEvent(new CustomizerUIEvents.UpdateChannelFabric(CustomizationChannel.RED, dragTexture, vector));
			CustomizationContext.EventBus.DispatchEvent(new CustomizerUIEvents.InputOverChannel(CustomizationChannel.RED));
			channelUpdated = CustomizationChannel.RED;
			fabricChannelData.UpdatedChannel = dragTexture;
		}

		protected virtual void dragChannelGreen(Vector2 textureCoord, SkinnedMeshRenderer smr)
		{
			Vector2 vector = DragChannel(CustomizationChannel.GREEN, textureCoord, fabricChannelData.UVOffsetGreen);
			fabricChannelData.UVOffsetGreen = vector;
			CustomizationContext.EventBus.DispatchEvent(new CustomizerUIEvents.UpdateChannelFabric(CustomizationChannel.RED, fabricChannelData.OriginalChannelRed, fabricChannelData.UVOffsetRed));
			CustomizationContext.EventBus.DispatchEvent(new CustomizerUIEvents.UpdateChannelFabric(CustomizationChannel.BLUE, fabricChannelData.OriginalChannelBlue, fabricChannelData.UVOffsetBlue));
			CustomizationContext.EventBus.DispatchEvent(new CustomizerUIEvents.UpdateChannelFabric(CustomizationChannel.GREEN, dragTexture, vector));
			CustomizationContext.EventBus.DispatchEvent(new CustomizerUIEvents.InputOverChannel(CustomizationChannel.GREEN));
			channelUpdated = CustomizationChannel.GREEN;
			fabricChannelData.UpdatedChannel = dragTexture;
		}

		protected virtual void dragChannelBlue(Vector2 textureCoord, SkinnedMeshRenderer smr)
		{
			Vector2 vector = DragChannel(CustomizationChannel.BLUE, textureCoord, fabricChannelData.UVOffsetBlue);
			fabricChannelData.UVOffsetBlue = vector;
			CustomizationContext.EventBus.DispatchEvent(new CustomizerUIEvents.UpdateChannelFabric(CustomizationChannel.RED, fabricChannelData.OriginalChannelRed, fabricChannelData.UVOffsetRed));
			CustomizationContext.EventBus.DispatchEvent(new CustomizerUIEvents.UpdateChannelFabric(CustomizationChannel.GREEN, fabricChannelData.OriginalChannelGreen, fabricChannelData.UVOffsetGreen));
			CustomizationContext.EventBus.DispatchEvent(new CustomizerUIEvents.UpdateChannelFabric(CustomizationChannel.BLUE, dragTexture, vector));
			CustomizationContext.EventBus.DispatchEvent(new CustomizerUIEvents.InputOverChannel(CustomizationChannel.BLUE));
			channelUpdated = CustomizationChannel.BLUE;
			fabricChannelData.UpdatedChannel = dragTexture;
		}

		protected virtual void dragChannelNone()
		{
			CustomizationContext.EventBus.DispatchEvent(new CustomizerUIEvents.UpdateChannelFabric(CustomizationChannel.GREEN, fabricChannelData.OriginalChannelGreen, fabricChannelData.UVOffsetGreen));
			CustomizationContext.EventBus.DispatchEvent(new CustomizerUIEvents.UpdateChannelFabric(CustomizationChannel.BLUE, fabricChannelData.OriginalChannelBlue, fabricChannelData.UVOffsetBlue));
			CustomizationContext.EventBus.DispatchEvent(new CustomizerUIEvents.UpdateChannelFabric(CustomizationChannel.RED, fabricChannelData.OriginalChannelRed, fabricChannelData.UVOffsetRed));
			CustomizationContext.EventBus.DispatchEvent(new CustomizerUIEvents.InputOverChannel(CustomizationChannel.NONE));
			channelUpdated = CustomizationChannel.NONE;
			lastChannel = CustomizationChannel.NONE;
		}

		private Vector2 DragChannel(CustomizationChannel channel, Vector2 inputCoord, Vector2 channelCoord)
		{
			Vector2 a = CustomizerConstants.TEXTURE_CENTER_UV - inputCoord;
			Vector2 b = a - lastUVPosition;
			if (lastChannel != channel)
			{
				b = Vector2.zero;
				lastChannel = channel;
			}
			lastUVPosition = a;
			Vector2 result = channelCoord + b;
			while (result.x > 1f)
			{
				result.x -= 2f;
			}
			while (result.x < -1f)
			{
				result.x += 2f;
			}
			while (result.y > 1f)
			{
				result.y -= 2f;
			}
			while (result.y < -1f)
			{
				result.y += 2f;
			}
			return result;
		}

		protected virtual void onDragEnd()
		{
			switch (channelUpdated)
			{
			case CustomizationChannel.RED:
				fabricChannelData.ActualChannelRed = fabricChannelData.UpdatedChannel;
				CustomizationContext.EventBus.DispatchEvent(default(CustomizerUIEvents.ShowCustomizationControls));
				CustomizationContext.EventBus.DispatchEvent(default(CustomizerUIEvents.OnApplyFabric));
				EventManager.Instance.PostEvent("SFX/UI/ClothingDesigner/ItemDrop", EventAction.PlaySound);
				break;
			case CustomizationChannel.GREEN:
				fabricChannelData.ActualChannelGreen = fabricChannelData.UpdatedChannel;
				CustomizationContext.EventBus.DispatchEvent(default(CustomizerUIEvents.ShowCustomizationControls));
				CustomizationContext.EventBus.DispatchEvent(default(CustomizerUIEvents.OnApplyFabric));
				EventManager.Instance.PostEvent("SFX/UI/ClothingDesigner/ItemDrop", EventAction.PlaySound);
				break;
			case CustomizationChannel.BLUE:
				fabricChannelData.ActualChannelBlue = fabricChannelData.UpdatedChannel;
				CustomizationContext.EventBus.DispatchEvent(default(CustomizerUIEvents.ShowCustomizationControls));
				CustomizationContext.EventBus.DispatchEvent(default(CustomizerUIEvents.OnApplyFabric));
				EventManager.Instance.PostEvent("SFX/UI/ClothingDesigner/ItemDrop", EventAction.PlaySound);
				break;
			default:
				CustomizationContext.EventBus.DispatchEvent(default(CustomizerDragEvents.EndDragFabricButton));
				break;
			}
			lastChannel = CustomizationChannel.NONE;
			CustomizationContext.EventBus.DispatchEvent(default(CustomizerDragEvents.GestureComplete));
			CustomizationContext.EventBus.DispatchEvent(new CustomizerUIEvents.InputStateChange(false, false));
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
