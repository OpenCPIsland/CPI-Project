using Fabric;
using UnityEngine;

namespace ClubPenguin.ClothingDesigner.ItemCustomizer
{
	internal class DragDecalButtonState : DragFabricButtonState
	{
		protected OriginalDecalData decalChannelData;

		public DragDecalButtonState(GameObject dragArea, Camera mainCamera, Camera guiCamera, DragContainer dragContainer)
			: base(dragArea, mainCamera, guiCamera, dragContainer)
		{
		}

		public void SetPersistentChannelData(OriginalDecalData decalChannelData)
		{
			this.decalChannelData = decalChannelData;
			channelUpdated = CustomizationChannel.NONE;
		}

		public override void EnterState(CustomizerGestureModel currentGesture)
		{
			decalChannelData.OriginalChannelRed = decalChannelData.ActualChannelRed;
			decalChannelData.OriginalChannelGreen = decalChannelData.ActualChannelGreen;
			decalChannelData.OriginalChannelBlue = decalChannelData.ActualChannelBlue;
			decalChannelData.OriginalRedUVOffset = decalChannelData.ActualRedUVOffset;
			decalChannelData.OriginalGreenUVOffset = decalChannelData.ActualGreenUVOffset;
			decalChannelData.OriginalBlueUVOffset = decalChannelData.ActualBlueUVOffset;
			decalChannelData.OriginalRedRenderer = decalChannelData.ActualRedRenderer;
			decalChannelData.OriginalGreenRenderer = decalChannelData.ActualGreenRenderer;
			decalChannelData.OriginalBlueRenderer = decalChannelData.ActualBlueRenderer;
			dragTexture = currentGesture.DragIconTexture;
			if (dragTexture != null)
			{
				CustomizationContext.EventBus.DispatchEvent(new CustomizerDragEvents.StartDragDecalButton(currentGesture));
				CustomizationContext.EventBus.DispatchEvent(new CustomizerUIEvents.InputStateChange(true, false));
				CustomizationContext.EventBus.DispatchEvent(new CustomizerUIEvents.InputOverChannel(CustomizationChannel.NONE));
				setupDragIcon(currentGesture);
				EventManager.Instance.PostEvent("SFX/UI/ClothingDesigner/ItemSelect", EventAction.PlaySound);
			}
			else
			{
				CustomizationContext.EventBus.DispatchEvent(default(CustomizerDragEvents.EndDragDecalButton));
				CustomizationContext.EventBus.DispatchEvent(default(CustomizerDragEvents.GestureComplete));
				CustomizationContext.EventBus.DispatchEvent(new CustomizerUIEvents.InputStateChange(false, false));
			}
		}

		protected override void dragChannelRed(Vector2 textureCoord, SkinnedMeshRenderer smr)
		{
			Vector2 vector = CustomizerConstants.TEXTURE_CENTER_UV - textureCoord;
			CustomizationContext.EventBus.DispatchEvent(new CustomizerUIEvents.UpdateChannelDecal(CustomizationChannel.GREEN, decalChannelData.OriginalChannelGreen, decalChannelData.OriginalGreenUVOffset, decalChannelData.OriginalGreenRenderer));
			CustomizationContext.EventBus.DispatchEvent(new CustomizerUIEvents.UpdateChannelDecal(CustomizationChannel.BLUE, decalChannelData.OriginalChannelBlue, decalChannelData.OriginalBlueUVOffset, decalChannelData.OriginalBlueRenderer));
			CustomizationContext.EventBus.DispatchEvent(new CustomizerUIEvents.UpdateChannelDecal(CustomizationChannel.RED, dragTexture, vector, smr));
			CustomizationContext.EventBus.DispatchEvent(new CustomizerUIEvents.InputOverChannel(CustomizationChannel.RED));
			channelUpdated = CustomizationChannel.RED;
			decalChannelData.UpdatedChannel = dragTexture;
			decalChannelData.UpdatedUVOffset = vector;
			decalChannelData.UpdatedRenderer = smr;
		}

		protected override void dragChannelGreen(Vector2 textureCoord, SkinnedMeshRenderer smr)
		{
			Vector2 vector = CustomizerConstants.TEXTURE_CENTER_UV - textureCoord;
			CustomizationContext.EventBus.DispatchEvent(new CustomizerUIEvents.UpdateChannelDecal(CustomizationChannel.RED, decalChannelData.OriginalChannelRed, decalChannelData.OriginalRedUVOffset, decalChannelData.OriginalRedRenderer));
			CustomizationContext.EventBus.DispatchEvent(new CustomizerUIEvents.UpdateChannelDecal(CustomizationChannel.BLUE, decalChannelData.OriginalChannelBlue, decalChannelData.OriginalBlueUVOffset, decalChannelData.OriginalBlueRenderer));
			CustomizationContext.EventBus.DispatchEvent(new CustomizerUIEvents.UpdateChannelDecal(CustomizationChannel.GREEN, dragTexture, vector, smr));
			CustomizationContext.EventBus.DispatchEvent(new CustomizerUIEvents.InputOverChannel(CustomizationChannel.GREEN));
			channelUpdated = CustomizationChannel.GREEN;
			decalChannelData.UpdatedChannel = dragTexture;
			decalChannelData.UpdatedUVOffset = vector;
			decalChannelData.UpdatedRenderer = smr;
		}

		protected override void dragChannelBlue(Vector2 textureCoord, SkinnedMeshRenderer smr)
		{
			Vector2 vector = CustomizerConstants.TEXTURE_CENTER_UV - textureCoord;
			CustomizationContext.EventBus.DispatchEvent(new CustomizerUIEvents.UpdateChannelDecal(CustomizationChannel.GREEN, decalChannelData.OriginalChannelGreen, decalChannelData.OriginalGreenUVOffset, decalChannelData.OriginalGreenRenderer));
			CustomizationContext.EventBus.DispatchEvent(new CustomizerUIEvents.UpdateChannelDecal(CustomizationChannel.RED, decalChannelData.OriginalChannelRed, decalChannelData.OriginalRedUVOffset, decalChannelData.OriginalRedRenderer));
			CustomizationContext.EventBus.DispatchEvent(new CustomizerUIEvents.UpdateChannelDecal(CustomizationChannel.BLUE, dragTexture, vector, smr));
			CustomizationContext.EventBus.DispatchEvent(new CustomizerUIEvents.InputOverChannel(CustomizationChannel.BLUE));
			channelUpdated = CustomizationChannel.BLUE;
			decalChannelData.UpdatedChannel = dragTexture;
			decalChannelData.UpdatedUVOffset = vector;
			decalChannelData.UpdatedRenderer = smr;
		}

		protected override void dragChannelNone()
		{
			CustomizationContext.EventBus.DispatchEvent(new CustomizerUIEvents.UpdateChannelDecal(CustomizationChannel.GREEN, decalChannelData.OriginalChannelGreen, decalChannelData.OriginalGreenUVOffset, decalChannelData.OriginalGreenRenderer));
			CustomizationContext.EventBus.DispatchEvent(new CustomizerUIEvents.UpdateChannelDecal(CustomizationChannel.RED, decalChannelData.OriginalChannelRed, decalChannelData.OriginalRedUVOffset, decalChannelData.OriginalRedRenderer));
			CustomizationContext.EventBus.DispatchEvent(new CustomizerUIEvents.UpdateChannelDecal(CustomizationChannel.BLUE, decalChannelData.OriginalChannelBlue, decalChannelData.OriginalBlueUVOffset, decalChannelData.OriginalBlueRenderer));
			CustomizationContext.EventBus.DispatchEvent(new CustomizerUIEvents.InputOverChannel(CustomizationChannel.NONE));
			channelUpdated = CustomizationChannel.NONE;
			decalChannelData.UpdatedChannel = null;
			decalChannelData.UpdatedUVOffset = Vector2.zero;
			decalChannelData.UpdatedRenderer = null;
		}

		protected override void onDragEnd()
		{
			switch (channelUpdated)
			{
			case CustomizationChannel.RED:
				decalChannelData.ActualChannelRed = decalChannelData.UpdatedChannel;
				decalChannelData.ActualRedUVOffset = decalChannelData.UpdatedUVOffset;
				decalChannelData.ActualRedRenderer = decalChannelData.UpdatedRenderer;
				CustomizationContext.EventBus.DispatchEvent(default(CustomizerUIEvents.ShowCustomizationControls));
				CustomizationContext.EventBus.DispatchEvent(default(CustomizerUIEvents.OnApplyDecal));
				EventManager.Instance.PostEvent("SFX/UI/ClothingDesigner/ItemDrop", EventAction.PlaySound);
				break;
			case CustomizationChannel.GREEN:
				decalChannelData.ActualChannelGreen = decalChannelData.UpdatedChannel;
				decalChannelData.ActualGreenUVOffset = decalChannelData.UpdatedUVOffset;
				decalChannelData.ActualGreenRenderer = decalChannelData.UpdatedRenderer;
				CustomizationContext.EventBus.DispatchEvent(default(CustomizerUIEvents.ShowCustomizationControls));
				CustomizationContext.EventBus.DispatchEvent(default(CustomizerUIEvents.OnApplyDecal));
				EventManager.Instance.PostEvent("SFX/UI/ClothingDesigner/ItemDrop", EventAction.PlaySound);
				break;
			case CustomizationChannel.BLUE:
				decalChannelData.ActualChannelBlue = decalChannelData.UpdatedChannel;
				decalChannelData.ActualBlueUVOffset = decalChannelData.UpdatedUVOffset;
				decalChannelData.ActualBlueRenderer = decalChannelData.UpdatedRenderer;
				CustomizationContext.EventBus.DispatchEvent(default(CustomizerUIEvents.ShowCustomizationControls));
				CustomizationContext.EventBus.DispatchEvent(default(CustomizerUIEvents.OnApplyDecal));
				EventManager.Instance.PostEvent("SFX/UI/ClothingDesigner/ItemDrop", EventAction.PlaySound);
				break;
			default:
				CustomizationContext.EventBus.DispatchEvent(default(CustomizerDragEvents.EndDragDecalButton));
				break;
			}
			CustomizationContext.EventBus.DispatchEvent(default(CustomizerDragEvents.GestureComplete));
			CustomizationContext.EventBus.DispatchEvent(new CustomizerUIEvents.InputStateChange(false, false));
		}
	}
}
