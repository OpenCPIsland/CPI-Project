using UnityEngine;

namespace ClubPenguin.ClothingDesigner.ItemCustomizer
{
	internal class DragFabricState : DragFabricButtonState
	{
		private CustomizationChannel startingChannel;

		public DragFabricState(GameObject dragArea, Camera mainCamera, Camera guiCamera, DragContainer dragContainer)
			: base(dragArea, mainCamera, guiCamera, dragContainer)
		{
		}

		public override void EnterState(CustomizerGestureModel currentGesture)
		{
			switch (currentGesture.Channel)
			{
			case CustomizationChannel.RED:
				fabricChannelData.OriginalChannelGreen = fabricChannelData.ActualChannelGreen;
				fabricChannelData.OriginalChannelBlue = fabricChannelData.ActualChannelBlue;
				fabricChannelData.OriginalChannelRed = null;
				currentGesture.DragIconTexture = fabricChannelData.ActualChannelRed;
				startingChannel = CustomizationChannel.RED;
				break;
			case CustomizationChannel.GREEN:
				fabricChannelData.OriginalChannelRed = fabricChannelData.ActualChannelRed;
				fabricChannelData.OriginalChannelBlue = fabricChannelData.ActualChannelBlue;
				fabricChannelData.OriginalChannelGreen = null;
				currentGesture.DragIconTexture = fabricChannelData.ActualChannelGreen;
				startingChannel = CustomizationChannel.GREEN;
				break;
			case CustomizationChannel.BLUE:
				fabricChannelData.OriginalChannelRed = fabricChannelData.ActualChannelRed;
				fabricChannelData.OriginalChannelGreen = fabricChannelData.ActualChannelGreen;
				fabricChannelData.OriginalChannelBlue = null;
				currentGesture.DragIconTexture = fabricChannelData.ActualChannelBlue;
				startingChannel = CustomizationChannel.BLUE;
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
			if (channelUpdated != startingChannel)
			{
				switch (channelUpdated)
				{
				case CustomizationChannel.RED:
					fabricChannelData.ActualChannelRed = fabricChannelData.UpdatedChannel;
					CustomizationContext.EventBus.DispatchEvent(default(CustomizerUIEvents.ShowCustomizationControls));
					break;
				case CustomizationChannel.GREEN:
					fabricChannelData.ActualChannelGreen = fabricChannelData.UpdatedChannel;
					CustomizationContext.EventBus.DispatchEvent(default(CustomizerUIEvents.ShowCustomizationControls));
					break;
				case CustomizationChannel.BLUE:
					fabricChannelData.ActualChannelBlue = fabricChannelData.UpdatedChannel;
					CustomizationContext.EventBus.DispatchEvent(default(CustomizerUIEvents.ShowCustomizationControls));
					break;
				}
				switch (startingChannel)
				{
				case CustomizationChannel.RED:
					fabricChannelData.ActualChannelRed = null;
					break;
				case CustomizationChannel.GREEN:
					fabricChannelData.ActualChannelGreen = null;
					break;
				case CustomizationChannel.BLUE:
					fabricChannelData.ActualChannelBlue = null;
					break;
				}
			}
			CustomizationContext.EventBus.DispatchEvent(default(CustomizerDragEvents.GestureComplete));
			CustomizationContext.EventBus.DispatchEvent(new CustomizerUIEvents.InputStateChange(false, false));
		}
	}
}
