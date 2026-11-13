using UnityEngine;

namespace ClubPenguin.ClothingDesigner.ItemCustomizer
{
	internal class DragDecalState : DragDecalButtonState
	{
		private CustomizationChannel startingChannel;

		public DragDecalState(GameObject dragArea, Camera mainCamera, Camera guiCamera, DragContainer dragContainer)
			: base(dragArea, mainCamera, guiCamera, dragContainer)
		{
		}

		public override void EnterState(CustomizerGestureModel currentGesture)
		{
			decalChannelData.OriginalRedUVOffset = decalChannelData.ActualRedUVOffset;
			decalChannelData.OriginalGreenUVOffset = decalChannelData.ActualGreenUVOffset;
			decalChannelData.OriginalBlueUVOffset = decalChannelData.ActualBlueUVOffset;
			decalChannelData.OriginalRedRenderer = decalChannelData.ActualRedRenderer;
			decalChannelData.OriginalGreenRenderer = decalChannelData.ActualGreenRenderer;
			decalChannelData.OriginalBlueRenderer = decalChannelData.ActualBlueRenderer;
			switch (currentGesture.Channel)
			{
			case CustomizationChannel.BLUE:
				decalChannelData.OriginalChannelRed = decalChannelData.ActualChannelRed;
				decalChannelData.OriginalChannelGreen = decalChannelData.ActualChannelGreen;
				decalChannelData.OriginalChannelBlue = null;
				decalChannelData.OriginalBlueRenderer = null;
				currentGesture.DragIconTexture = decalChannelData.ActualChannelBlue;
				startingChannel = CustomizationChannel.BLUE;
				break;
			case CustomizationChannel.GREEN:
				decalChannelData.OriginalChannelRed = decalChannelData.ActualChannelRed;
				decalChannelData.OriginalChannelBlue = decalChannelData.ActualChannelBlue;
				decalChannelData.OriginalChannelGreen = null;
				decalChannelData.OriginalGreenRenderer = null;
				currentGesture.DragIconTexture = decalChannelData.ActualChannelGreen;
				startingChannel = CustomizationChannel.GREEN;
				break;
			case CustomizationChannel.RED:
				decalChannelData.OriginalChannelGreen = decalChannelData.ActualChannelGreen;
				decalChannelData.OriginalChannelBlue = decalChannelData.ActualChannelBlue;
				decalChannelData.OriginalChannelRed = null;
				decalChannelData.OriginalRedRenderer = null;
				currentGesture.DragIconTexture = decalChannelData.ActualChannelRed;
				startingChannel = CustomizationChannel.RED;
				break;
			}
			if (currentGesture.DragIconTexture != null)
			{
				dragTexture = currentGesture.DragIconTexture;
				setupDragIcon(currentGesture);
				CustomizationContext.EventBus.DispatchEvent(new CustomizerUIEvents.InputStateChange(true, false));
				CustomizationContext.EventBus.DispatchEvent(new CustomizerUIEvents.InputOverChannel(CustomizationChannel.NONE));
			}
			else
			{
				CustomizationContext.EventBus.DispatchEvent(default(CustomizerDragEvents.GestureComplete));
				CustomizationContext.EventBus.DispatchEvent(new CustomizerUIEvents.InputStateChange(false, false));
			}
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
				break;
			case CustomizationChannel.GREEN:
				decalChannelData.ActualChannelGreen = decalChannelData.UpdatedChannel;
				decalChannelData.ActualGreenUVOffset = decalChannelData.UpdatedUVOffset;
				decalChannelData.ActualGreenRenderer = decalChannelData.UpdatedRenderer;
				CustomizationContext.EventBus.DispatchEvent(default(CustomizerUIEvents.ShowCustomizationControls));
				break;
			case CustomizationChannel.BLUE:
				decalChannelData.ActualChannelBlue = decalChannelData.UpdatedChannel;
				decalChannelData.ActualBlueUVOffset = decalChannelData.UpdatedUVOffset;
				decalChannelData.ActualBlueRenderer = decalChannelData.UpdatedRenderer;
				CustomizationContext.EventBus.DispatchEvent(default(CustomizerUIEvents.ShowCustomizationControls));
				break;
			}
			if (channelUpdated != startingChannel)
			{
				switch (startingChannel)
				{
				case CustomizationChannel.RED:
					decalChannelData.ActualChannelRed = null;
					decalChannelData.ActualRedRenderer = null;
					break;
				case CustomizationChannel.GREEN:
					decalChannelData.ActualChannelGreen = null;
					decalChannelData.ActualGreenRenderer = null;
					break;
				case CustomizationChannel.BLUE:
					decalChannelData.ActualChannelBlue = null;
					decalChannelData.ActualBlueRenderer = null;
					break;
				}
			}
			CustomizationContext.EventBus.DispatchEvent(default(CustomizerDragEvents.GestureComplete));
			CustomizationContext.EventBus.DispatchEvent(new CustomizerUIEvents.InputStateChange(false, false));
		}
	}
}
