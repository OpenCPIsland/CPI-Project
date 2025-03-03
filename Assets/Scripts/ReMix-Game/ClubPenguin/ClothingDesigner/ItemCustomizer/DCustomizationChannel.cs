using CpRemixShaders;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.ClothingDesigner.ItemCustomizer
{
	public class DCustomizationChannel
	{
		private enum FabricState
		{
			NO_FABRIC,
			FABRIC,
			CUSTOM_FABRIC
		}

		private enum DecalState
		{
			NO_DECAL,
			DECAL,
			CUSTOM_DECAL
		}

		public enum ChannelMask
		{
			RED,
			GREEN,
			BLUE
		}

		private FabricState currentFabricState;

		private DecalState currentDecalState;

		private bool isEnabled = true;

		private ChannelMask channelMask;

		private string channelName;

		public static readonly float DEFAULT_SCALE = 6.5f;

		public static readonly float DEFAULT_SCALE_DECAL = 6.5f;

		private Dictionary<CustomizationOption, bool> customizationOptions;

		private Texture2D _defaultTexture = null;

		private bool _canRotateAndScaleFabric = true;

		private Dictionary<string, bool> _fabricNameToInteractable = null;

		private Color color;

		private DCustomizerLayer fabricLayer;

		private DCustomizerLayer decalLayer;

		private bool isSelected = false;

		private bool canFade = false;

		private Color darkenedColor;

		public bool IsEnabled
		{
			get
			{
				return isEnabled;
			}
			set
			{
				isEnabled = value;
				CustomizationContext.EventBus.DispatchEvent(new CustomizerModelEvents.ChannelEnabledChangedEvent(this, isEnabled));
			}
		}

		public ChannelMask Mask
		{
			get
			{
				return channelMask;
			}
		}

		public string ChannelName
		{
			get
			{
				return channelName;
			}
		}

		public Color Color
		{
			get
			{
				return color;
			}
			set
			{
				if (currentFabricState == FabricState.FABRIC)
				{
					SetFabric(null);
				}
				else if (currentFabricState == FabricState.CUSTOM_FABRIC)
				{
					setCustomFabric(null);
				}
				color = value;
				CustomizationContext.EventBus.DispatchEvent(new CustomizerModelEvents.ColorChangedEvent(color, this));
			}
		}

		public Texture2D Fabric
		{
			get
			{
				return fabricLayer.Decal;
			}
		}

		public Color FabricTint
		{
			get
			{
				return fabricLayer.Tint;
			}
			set
			{
				fabricLayer.Tint = value;
				CustomizationContext.EventBus.DispatchEvent(new CustomizerModelEvents.DecalTintChangedEvent(fabricLayer.Tint, this, fabricLayer));
			}
		}

		public float FabricScale
		{
			get
			{
				return fabricLayer.Scale;
			}
			set
			{
				fabricLayer.Scale = value;
				CustomizationContext.EventBus.DispatchEvent(new CustomizerModelEvents.DecalScaledEvent(fabricLayer.Scale, this, fabricLayer));
			}
		}

		public float FabricRotation
		{
			get
			{
				return fabricLayer.Rotation;
			}
			set
			{
				fabricLayer.Rotation = value;
				CustomizationContext.EventBus.DispatchEvent(new CustomizerModelEvents.DecalRotatedEvent(fabricLayer.Rotation, this, fabricLayer));
			}
		}

		public Vector2 FabricUVOffset
		{
			get
			{
				return fabricLayer.UVOffset;
			}
			set
			{
				fabricLayer.UVOffset = value;
				CustomizationContext.EventBus.DispatchEvent(new CustomizerModelEvents.DecalMovedEvent(fabricLayer.UVOffset, this, fabricLayer));
			}
		}

		public Texture2D Decal
		{
			get
			{
				return decalLayer.Decal;
			}
		}

		public Color DecalTint
		{
			get
			{
				return decalLayer.Tint;
			}
			set
			{
				decalLayer.Tint = value;
				CustomizationContext.EventBus.DispatchEvent(new CustomizerModelEvents.DecalTintChangedEvent(decalLayer.Tint, this, decalLayer));
			}
		}

		public bool IsDecalTiled
		{
			get
			{
				return decalLayer.IsTiled;
			}
			set
			{
				decalLayer.IsTiled = value;
				CustomizationContext.EventBus.DispatchEvent(new CustomizerModelEvents.DecalTilingChangedEvent(decalLayer.IsTiled, this, decalLayer));
			}
		}

		public float DecalScale
		{
			get
			{
				return decalLayer.Scale;
			}
			set
			{
				decalLayer.Scale = value;
				CustomizationContext.EventBus.DispatchEvent(new CustomizerModelEvents.DecalScaledEvent(decalLayer.Scale, this, decalLayer));
			}
		}

		public float DecalRotation
		{
			get
			{
				return decalLayer.Rotation;
			}
			set
			{
				decalLayer.Rotation = value;
				CustomizationContext.EventBus.DispatchEvent(new CustomizerModelEvents.DecalRotatedEvent(decalLayer.Rotation, this, decalLayer));
			}
		}

		public Vector2 DecalUVOffset
		{
			get
			{
				return decalLayer.UVOffset;
			}
			set
			{
				decalLayer.UVOffset = value;
				CustomizationContext.EventBus.DispatchEvent(new CustomizerModelEvents.DecalMovedEvent(decalLayer.UVOffset, this, decalLayer));
			}
		}

		public Renderer DecalRenderer
		{
			get
			{
				return decalLayer.OriginalRenderer;
			}
			set
			{
				decalLayer.OriginalRenderer = value;
			}
		}

		public bool IsSelected
		{
			get
			{
				return isSelected;
			}
		}

		public DCustomizationChannel(ChannelMask channelColor)
		{
			customizationOptions = new Dictionary<CustomizationOption, bool>();
			customizationOptions[CustomizationOption.FABRIC] = false;
			customizationOptions[CustomizationOption.DECAL] = false;
			channelName = channelColor.ToString();
			channelMask = channelColor;
			currentFabricState = FabricState.NO_FABRIC;
			fabricLayer = new DCustomizerLayer();
			fabricLayer.Type = DecalType.FABRIC;
			fabricLayer.Scale = DEFAULT_SCALE;
			fabricLayer.IsTiled = true;
			currentDecalState = DecalState.NO_DECAL;
			decalLayer = new DCustomizerLayer();
			decalLayer.Type = DecalType.DECAL;
			decalLayer.Scale = DEFAULT_SCALE_DECAL;
			decalLayer.IsTiled = false;
			DecalRenderer = null;
			switch (channelMask)
			{
			case ChannelMask.RED:
				fabricLayer.ShaderChannel = DecalColorChannel.RED_1;
				decalLayer.ShaderChannel = DecalColorChannel.RED_4;
				break;
			case ChannelMask.GREEN:
				fabricLayer.ShaderChannel = DecalColorChannel.GREEN_2;
				decalLayer.ShaderChannel = DecalColorChannel.GREEN_5;
				break;
			case ChannelMask.BLUE:
				fabricLayer.ShaderChannel = DecalColorChannel.BLUE_3;
				decalLayer.ShaderChannel = DecalColorChannel.BLUE_6;
				break;
			}
		}

		public void SetOptionIsAvailable(CustomizationOption option, bool IsAvailable)
		{
			customizationOptions[option] = IsAvailable;
			CustomizationContext.EventBus.DispatchEvent(new CustomizerModelEvents.CustomizationOptionsChanged(this));
		}

		public bool IsCustomizationAvailable(CustomizationOption option)
		{
			return customizationOptions[option];
		}

		public void SetFabric(Texture2D texture2D)
		{
			updateFabric(texture2D, FabricState.FABRIC);
		}

		private void setCustomFabric(Texture2D texture2D)
		{
			updateFabric(texture2D, FabricState.CUSTOM_FABRIC);
		}

		private void updateFabric(Texture2D texture2D, FabricState fabricType)
		{
			if (texture2D == null)
			{
				currentFabricState = FabricState.NO_FABRIC;
				texture2D = _defaultTexture;
			}
			else
			{
				currentFabricState = fabricType;
			}
			fabricLayer.Decal = texture2D;
			CustomizationContext.EventBus.DispatchEvent(new CustomizerModelEvents.DecalChangedEvent(fabricLayer.Decal, this, fabricLayer));
			_canRotateAndScaleFabric = true;
			if (texture2D != null)
			{
				bool value = false;
				if (_fabricNameToInteractable != null && _fabricNameToInteractable.TryGetValue(texture2D.name, out value))
				{
					_canRotateAndScaleFabric = value;
				}
			}
			if (texture2D != null)
			{
				fabricLayer.Tint = Color.white;
				CustomizationContext.EventBus.DispatchEvent(new CustomizerModelEvents.DecalTintChangedEvent(fabricLayer.Tint, this, fabricLayer));
			}
		}

		public bool hasFabric()
		{
			return currentFabricState != FabricState.NO_FABRIC;
		}

		public bool canRotateAndScaleFabric()
		{
			return _canRotateAndScaleFabric;
		}

		public void SetDecal(Texture2D texture2D, Renderer chosenRenderer)
		{
			updateDecal(texture2D, DecalState.DECAL, chosenRenderer);
		}

		public void SetCustomDecal(Texture2D texture2D)
		{
			updateDecal(texture2D, DecalState.CUSTOM_DECAL);
		}

		private void updateDecal(Texture2D texture2D, DecalState decalType, Renderer chosenRenderer = null)
		{
			decalLayer.OriginalRenderer = chosenRenderer;
			if (texture2D == null)
			{
				currentDecalState = DecalState.NO_DECAL;
			}
			else
			{
				currentDecalState = decalType;
			}
			decalLayer.Decal = texture2D;
			CustomizationContext.EventBus.DispatchEvent(new CustomizerModelEvents.DecalChangedEvent(decalLayer.Decal, this, decalLayer, chosenRenderer));
			if (texture2D != null && currentDecalState == DecalState.CUSTOM_DECAL)
			{
				decalLayer.Tint = Color.white;
				CustomizationContext.EventBus.DispatchEvent(new CustomizerModelEvents.DecalTintChangedEvent(decalLayer.Tint, this, decalLayer));
			}
		}

		public bool hasDecal()
		{
			return currentDecalState == DecalState.DECAL;
		}

		public bool hasCustomDecal()
		{
			return currentDecalState == DecalState.CUSTOM_DECAL;
		}

		public void Select()
		{
			isSelected = true;
			UpdateColoring();
		}

		public void Deselect()
		{
			isSelected = false;
			UpdateColoring();
		}

		public void SetDefaultTexture(Texture2D texture)
		{
			_defaultTexture = texture;
		}

		public void SetFabricNameToInteractable(Dictionary<string, bool> fabricNameToInteractable)
		{
			_fabricNameToInteractable = fabricNameToInteractable;
		}

		public void SetSelectionColors(Color darkenedColor)
		{
			this.darkenedColor = darkenedColor;
		}

		public void SetCanFade(bool canFade)
		{
			this.canFade = canFade;
			UpdateColoring();
		}

		private void UpdateColoring()
		{
			Color white = Color.white;
			Color white2 = Color.white;
			if (canFade && !isSelected)
			{
				white = darkenedColor;
				white2 = darkenedColor;
			}
			FabricTint = white;
			DecalTint = white2;
		}
	}
}
