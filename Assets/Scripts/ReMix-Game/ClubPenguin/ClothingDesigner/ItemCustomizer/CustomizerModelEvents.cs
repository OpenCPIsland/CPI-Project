using System.Runtime.InteropServices;
using UnityEngine;

namespace ClubPenguin.ClothingDesigner.ItemCustomizer
{
	public static class CustomizerModelEvents
	{
		public struct ChannelChangedEvent
		{
			public readonly DCustomizationChannel NewChannel;

			public ChannelChangedEvent(DCustomizationChannel newChannel)
			{
				NewChannel = newChannel;
			}
		}

		public struct CustomizationOptionsChanged
		{
			public readonly DCustomizationChannel Channel;

			public CustomizationOptionsChanged(DCustomizationChannel channel)
			{
				Channel = channel;
			}
		}

		public struct ChannelEnabledChangedEvent
		{
			public readonly DCustomizationChannel Channel;

			public readonly bool IsEnabled;

			public ChannelEnabledChangedEvent(DCustomizationChannel channel, bool isEnabled)
			{
				Channel = channel;
				IsEnabled = isEnabled;
			}
		}

		public struct TemplateChangedEvent
		{
			public readonly TemplateDefinition Definition;

			public TemplateChangedEvent(TemplateDefinition definition)
			{
				Definition = definition;
			}
		}

		public struct ColorChangedEvent
		{
			public readonly Color NewColor;

			public readonly DCustomizationChannel Channel;

			public ColorChangedEvent(Color newColor, DCustomizationChannel channel)
			{
				NewColor = newColor;
				Channel = channel;
			}
		}

		public struct DecalChangedEvent
		{
			public readonly Texture2D NewDecal;

			public readonly DCustomizationChannel Channel;

			public readonly DCustomizerLayer DecalLayer;

			public readonly Renderer ChosenRenderer;

			public DecalChangedEvent(Texture2D newDecal, DCustomizationChannel channel, DCustomizerLayer decalLayer, Renderer chosenRenderer = null)
			{
				NewDecal = newDecal;
				Channel = channel;
				DecalLayer = decalLayer;
				ChosenRenderer = chosenRenderer;
			}
		}

		public struct DecalTilingChangedEvent
		{
			public readonly bool IsTiled;

			public readonly DCustomizationChannel Channel;

			public readonly DCustomizerLayer DecalLayer;

			public DecalTilingChangedEvent(bool isTiled, DCustomizationChannel channel, DCustomizerLayer decalLayer)
			{
				IsTiled = isTiled;
				Channel = channel;
				DecalLayer = decalLayer;
			}
		}

		public struct DecalScaledEvent
		{
			public readonly float NewScale;

			public readonly DCustomizationChannel Channel;

			public readonly DCustomizerLayer DecalLayer;

			public DecalScaledEvent(float newScale, DCustomizationChannel channel, DCustomizerLayer decalLayer)
			{
				NewScale = newScale;
				Channel = channel;
				DecalLayer = decalLayer;
			}
		}

		public struct DecalRotatedEvent
		{
			public readonly float TotalRotation;

			public readonly DCustomizationChannel Channel;

			public readonly DCustomizerLayer DecalLayer;

			public DecalRotatedEvent(float totalRotation, DCustomizationChannel channel, DCustomizerLayer decalLayer)
			{
				TotalRotation = totalRotation;
				Channel = channel;
				DecalLayer = decalLayer;
			}
		}

		public struct DecalTintChangedEvent
		{
			public readonly Color NewColor;

			public readonly DCustomizationChannel Channel;

			public readonly DCustomizerLayer DecalLayer;

			public DecalTintChangedEvent(Color newColor, DCustomizationChannel channel, DCustomizerLayer decalLayer)
			{
				NewColor = newColor;
				Channel = channel;
				DecalLayer = decalLayer;
			}
		}

		public struct DecalMovedEvent
		{
			public readonly Vector2 UVOffset;

			public readonly DCustomizationChannel Channel;

			public readonly DCustomizerLayer DecalLayer;

			public DecalMovedEvent(Vector2 uvOffset, DCustomizationChannel channel, DCustomizerLayer decalLayer)
			{
				UVOffset = uvOffset;
				Channel = channel;
				DecalLayer = decalLayer;
			}
		}

		public struct CustomizerStateChangedEvent
		{
			public readonly CustomizerState NewState;

			public CustomizerStateChangedEvent(CustomizerState newState)
			{
				NewState = newState;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct ResetItemModelEvent
		{
		}
	}
}
