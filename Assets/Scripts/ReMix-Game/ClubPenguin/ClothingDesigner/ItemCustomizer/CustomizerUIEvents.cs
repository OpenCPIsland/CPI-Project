using ClubPenguin.Avatar;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace ClubPenguin.ClothingDesigner.ItemCustomizer
{
	public static class CustomizerUIEvents
	{
		public struct SelectChannelEvent
		{
			public readonly DCustomizationChannel NewChannel;

			public SelectChannelEvent(DCustomizationChannel newChannel)
			{
				NewChannel = newChannel;
			}
		}

		public struct SelectCustomizerStateEvent
		{
			public readonly DCustomizationChannel NewState;

			public SelectCustomizerStateEvent(DCustomizationChannel newState)
			{
				NewState = newState;
			}
		}

		public struct SelectTemplate
		{
			public readonly TemplateDefinition TemplateData;

			public readonly Sprite TemplateSprite;

			public SelectTemplate(TemplateDefinition templateData, Sprite templateSprite)
			{
				TemplateData = templateData;
				TemplateSprite = templateSprite;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct TemplateConfirmed
		{
		}

		public struct SelectColorEvent
		{
			public readonly Color NewColor;

			public SelectColorEvent(Color newColor)
			{
				NewColor = newColor;
			}
		}

		public struct SelectDecalEvent
		{
			public readonly Texture2D NewDecal;

			public SelectDecalEvent(Texture2D newDecal)
			{
				NewDecal = newDecal;
			}
		}

		public struct SelectDecalColorEvent
		{
			public readonly Color NewColor;

			public SelectDecalColorEvent(Color newColor)
			{
				NewColor = newColor;
			}
		}

		public struct UpdateChannelFabric
		{
			public readonly Texture2D NewFabric;

			public readonly CustomizationChannel ChannelToUpdate;

			public readonly Vector2 UVOffset;

			public UpdateChannelFabric(CustomizationChannel channelToUpdate, Texture2D newFabric, Vector2 uvOffset)
			{
				NewFabric = newFabric;
				ChannelToUpdate = channelToUpdate;
				UVOffset = uvOffset;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct OnApplyFabric
		{
		}

		public struct UpdateChannelDecal
		{
			public readonly Texture2D NewDecal;

			public readonly CustomizationChannel ChannelToUpdate;

			public readonly Vector2 UVOffset;

			public readonly Renderer ChosenRenderer;

			public UpdateChannelDecal(CustomizationChannel channelToUpdate, Texture2D newDecal, Vector2 uvOffset, Renderer chosenRenderer = null)
			{
				NewDecal = newDecal;
				ChannelToUpdate = channelToUpdate;
				UVOffset = uvOffset;
				ChosenRenderer = chosenRenderer;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct OnApplyDecal
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct ConfirmSaveClickedEvent
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct CloseEvent
		{
		}

		public struct BackButtonClickedEvent
		{
			public bool ShowConfirmation;

			public BackButtonClickedEvent(bool showConfirmation)
			{
				ShowConfirmation = showConfirmation;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct SaveClothingItemSuccess
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct SaveClothingItemError
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct SaveCancel
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct SaveItem
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct SaveItemFailure
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct ResetRotation
		{
		}

		public struct ItemZoomOffsetUpdated
		{
			public readonly float ZoomOffset;

			public ItemZoomOffsetUpdated(float zoomOffset)
			{
				ZoomOffset = zoomOffset;
			}
		}

		public struct ItemRotationYOffsetUpdated
		{
			public readonly float RotationYOffset;

			public ItemRotationYOffsetUpdated(float rotationYOffset)
			{
				RotationYOffset = rotationYOffset;
			}
		}

		public struct CameraZoomInOnGameObject
		{
			public readonly Transform ObjectTransform;

			public readonly List<MeshCollider> ObjectMeshColliders;

			public readonly float ZoomOffset;

			public CameraZoomInOnGameObject(Transform objectTransform, List<MeshCollider> meshColliders, float zoomOffset)
			{
				ObjectTransform = objectTransform;
				ObjectMeshColliders = meshColliders;
				ZoomOffset = zoomOffset;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct ShowCustomizationControls
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct EnableScrollRect
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct DisableScrollRect
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct SwitchToCustomize
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct SwitchToSave
		{
		}

		public struct InputStateChange
		{
			public readonly bool IsDown;

			public readonly bool IsManipulator;

			public InputStateChange(bool isDown, bool isManipulator)
			{
				IsDown = isDown;
				IsManipulator = isManipulator;
			}
		}

		public struct InputOverChannel
		{
			public readonly CustomizationChannel Channel;

			public InputOverChannel(CustomizationChannel channel)
			{
				Channel = channel;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct StartPurchaseMoment
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct EndPurchaseMoment
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct ConfirmSubmitClickedEvent
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct SubmitClothingItemError
		{
		}

		public struct SubmitClothingItemStart
		{
			public readonly DCustomEquipment SubmittedItem;

			public SubmitClothingItemStart(DCustomEquipment submittedItem)
			{
				SubmittedItem = submittedItem;
			}
		}
	}
}
